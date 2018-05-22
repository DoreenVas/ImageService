using ImageService.Infrastructure;
using ImageService.Logging;
using ImageService.Logging.Modal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Communication.Server
{
    public class TcpServerChannel : IServerCommunicationChannel
    {
        private int port;
        private TcpListener tcpListener;
        private ILoggingService loggingService;
        private IClientHandler clientHandler;
        private List<TcpClient> tcpClients = new List<TcpClient>();
        private object writeLock;
        private object listLock = new object();

        public TcpServerChannel(int port, ILoggingService loggingService, IClientHandler clientHandler)
        {
            this.port = port;
            this.clientHandler = clientHandler;
            this.loggingService = loggingService;
            writeLock = clientHandler.getLock();
        }

        public void Start()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start();
            loggingService.Log("Created a new TCP server channel.", MessageTypeEnum.INFO); 
            loggingService.Log("Waiting for connections...", MessageTypeEnum.INFO); 

            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        loggingService.Log("Established a new connection ",MessageTypeEnum.INFO);
                        lock (listLock)
                        {
                            tcpClients.Add(client);
                        }
                        clientHandler.HandleClient(client, tcpClients);
                    }
                    catch (SocketException se)
                    {
                       
                        loggingService.Log(se.ToString(), MessageTypeEnum.FAIL); 
                        break;
                    }
                }
                loggingService.Log("Server stopped", MessageTypeEnum.WARNING);
            });
            task.Start();
        }

        public void Stop()
        {
            /*foreach (TcpClient client in m_tcpClients)
            {
                client.Close();
            }*/
            tcpListener.Stop();
        }

        
        public void NotifyClients(CommandRecievedEventArgs commandRecievedEventArgs)
        {
            try
            {
                List<TcpClient> tcpClientsCopiedList = new List<TcpClient>(tcpClients);
                foreach (TcpClient tcpClient in tcpClientsCopiedList)
                {
                    new Task(() =>
                    {
                        try
                        {
                            NetworkStream stream = tcpClient.GetStream();
                            BinaryWriter writer = new BinaryWriter(stream);
                            string jsonCommand = JsonConvert.SerializeObject(commandRecievedEventArgs);
                            lock (writeLock)
                            {
                                writer.Write(jsonCommand);
                            }
                        }
                        catch (Exception)
                        {            
                            tcpClients.Remove(tcpClient);
                        }
                    }).Start();
                }
            }
            catch (Exception exc)
            {
                loggingService.Log(exc.ToString(), MessageTypeEnum.FAIL);
            }
        }   

    }
}
