﻿using ImageService.Communication.Server;
using ImageService.Controller;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.ClientHandler
{
    class HandleGuiClient: IClientHandler
    {
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        public event NotifyClients NotifyClients;

        private IImageController m_controller;
        private ILoggingService m_logging;
        private ImageServer m_imageServer;

        public HandleGuiClient(IImageController imageController, ILoggingService loggingService,ImageServer imageServer)
        {
            m_controller = imageController;
            m_logging = loggingService;
            m_logging.MessageRecieved += NewLogCommand;
            m_imageServer = imageServer;
        }

        private object writeLock = new object();

        public object getLock()
        {
            return writeLock;
        }

        public void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            new Task(() =>
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);
                    BinaryWriter writer = new BinaryWriter(stream);

                    while (client.Connected)
                    {
                        string command = reader.ReadString();
                        if (command == null)
                            continue;
                        CommandRecievedEventArgs commandRecievedEventArgs = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(command);
                        m_logging.Log("HandleClient got the command: " + (CommandEnum)commandRecievedEventArgs.CommandID, MessageTypeEnum.INFO);
                        if (commandRecievedEventArgs.CommandID == (int)CommandEnum.ClientClosedCommand)
                        {
                            clients.Remove(client);
                            client.Close();
                            m_logging.Log("A client was removed ", MessageTypeEnum.INFO);
                            break;
                        }
                        else if (commandRecievedEventArgs.CommandID == (int)CommandEnum.CloseCommand)
                        {
                            CommandRecieved?.Invoke(this, commandRecievedEventArgs);
                            if (m_imageServer.Handlers.Contains(commandRecievedEventArgs.RequestDirPath))
                                m_imageServer.Handlers.Remove(commandRecievedEventArgs.RequestDirPath);
                            Thread.Sleep(100);
                            string[] arr = new string[1];
                            arr[0] = commandRecievedEventArgs.RequestDirPath;
                            CommandRecievedEventArgs command2 = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, arr, "");
                            NotifyClients?.Invoke(command2);
                            continue;
                        }
                        else if (commandRecievedEventArgs.CommandID == (int)CommandEnum.GetConfigCommand)
                        {
                            string handlers = "";
                            foreach (string handler in m_imageServer.Handlers)
                            {
                                handlers += handler + ";";
                            }
                            if (handlers != "")
                                handlers.TrimEnd(';');
                            commandRecievedEventArgs.Args[0] = handlers;
                        }
                        bool success;
                        string msg = m_controller.ExecuteCommand(commandRecievedEventArgs.CommandID, commandRecievedEventArgs.Args, out success);
                        if (success)
                        {
                            m_logging.Log("Success executing command: " + (CommandEnum)commandRecievedEventArgs.CommandID, MessageTypeEnum.INFO);
                        }
                        else
                        {
                            m_logging.Log(msg, MessageTypeEnum.FAIL);
                        }
                        lock (writeLock)
                        {
                            writer.Write(msg);
                        }
                    }
                }
                catch (Exception exc)
                {
                    clients.Remove(client);
                    client.Close();
                    m_logging.Log(exc.ToString(), MessageTypeEnum.FAIL);
                }
            }).Start();
        }

        /// <summary>
        /// Activates NotifyClients event if a new log has arrived.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLogCommand(object sender, MessageRecievedEventArgs e)
        {
            string jsonCommand = JsonConvert.SerializeObject(e);
            string[] arr = new string[1];
            arr[0] = jsonCommand;
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.NewLogCommand, arr, "");
            NotifyClients?.Invoke(command);
        }

    }
}
