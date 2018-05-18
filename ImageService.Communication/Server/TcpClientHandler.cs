using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Communication.Server
{
    public class TcpClientHandler : IClientHandler
    {
        //private IImageController m_imageController;
        //private ILoggingService m_loggingService;
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        private Mutex mutexLock = new Mutex(); // todo: check if OK that mutex is private.

        public TcpClientHandler(/*IImageController a_imageController, ILoggingService a_loggingService*/)
        {
            //m_imageController = a_imageController;
            //m_loggingService = a_loggingService;

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
