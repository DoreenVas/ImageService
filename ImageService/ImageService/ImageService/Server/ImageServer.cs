using ImageService.Communication.Server;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Server
{
    /// <summary>
    /// Represents an image server.
    /// </summary>
    public class ImageServer : IClientHandler
    {    
        private IImageController m_controller;
        private ILoggingService m_logging;
        private string m_handler;

        //Todo: needed for more functions (beside handleclient)?
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        private Mutex mutexLock = new Mutex();

        public event EventHandler<CommandRecievedEventArgs> CommandRecieved; // The event that notifies about a new Command being recieved

        /// <summary>
        /// Constructor. creates a new image server instance.
        /// </summary>
        /// <param name="imageController">The image controller.</param> 
        /// <param name="loggingService">The logging service.</param> 
        /// <param name="handler">The paths to be handled separated by semicolons.</param>  
        public ImageServer(IImageController imageController, ILoggingService loggingService, string handler)
        {
            m_controller = imageController;
            m_logging = loggingService;
            m_handler = handler;

            string[] paths = m_handler.Split(';');
            foreach (var path in paths)
            {
                CreateDirectoryHandler(path);
            }
        }
        
        /// <summary>
        /// Creates a new directory handler for the directory of the given path.
        /// </summary>
        /// <param name="path">The path of the directory to be handled.</param> 
        private void CreateDirectoryHandler(string path)
        {
            IDirectoryHandler directoryHandler = new DirectoyHandler(m_controller, m_logging);
            m_logging.Log("A directory handler was created for the directory in path: " + path, Logging.Modal.MessageTypeEnum.INFO);
            CommandRecieved += directoryHandler.OnCommandRecieved;
            directoryHandler.DirectoryClose += this.OnDirectoryClose;
            directoryHandler.StartHandleDirectory(path);
        }
        
        /// <summary>
        /// This method is called when a directory is closing.
        /// </summary>
        /// <param name="sender">The command sender.</param> 
        /// <param name="args">The directory close event arguments.</param> 
        private void OnDirectoryClose(object sender, DirectoryCloseEventArgs args)
        {
            if (sender is IDirectoryHandler)
            {
                IDirectoryHandler directoryHandler = (IDirectoryHandler)sender;
                CommandRecieved -= directoryHandler.OnCommandRecieved;
                m_logging.Log(args.Message, Logging.Modal.MessageTypeEnum.INFO);
            }
        }

        /// <summary>
        /// Close the server.invoke the command of it closing.
        /// </summary>
        public void CloseServer()
        {
            string[] args = { };
            CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, args, "*");
            CommandRecieved?.Invoke(this, e);
            m_logging.Log("Server is Closing ", Logging.Modal.MessageTypeEnum.INFO);
        }

        public void HandleClient(TcpClient client, List<TcpClient> clients)
        {
            new Task(() =>
            {
                try
                {
                    stream = client.GetStream();
                    reader = new BinaryReader(stream);
                    writer = new BinaryWriter(stream);

                    while (client.Connected)
                    {
                        // todo: maybe add try catch block: starts here
                        string commandLine = reader.ReadString();
                        // todo: maybe add try catch block: ends here
                        //m_loggingService.Log(this.ToString() + " got the command " + commandLine + ".", Logging.Modal.MessageTypeEnum.INFO); // todo: change messagetypeenum.
                        CommandRecievedEventArgs commandRecievedEventArgs = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(commandLine);
                        if (commandRecievedEventArgs.CommandID == (int)CommandEnum.CloseCommand)
                        {
                            clients.Remove(client);
                            client.Close();
                            // todo: maybe add break here.
                        }
                        //Console.WriteLine("Got command: {0}", commandLine); 
                        bool result;
                        //string commandStr = m_imageController.ExecuteCommand((int)commandRecievedEventArgs.CommandID, commandRecievedEventArgs.Args, out result);
                        mutexLock.WaitOne();
                        //m_writer.Write(commandStr);
                        mutexLock.ReleaseMutex();
                    }
                }
                catch (Exception exception)
                {
                    clients.Remove(client);
                    client.Close(); // todo: should it be here?
                    //m_loggingService.Log(exception.ToString(), Logging.Modal.MessageTypeEnum.FAIL); // change message type enum
                }
            }).Start();
        }
    }
}
