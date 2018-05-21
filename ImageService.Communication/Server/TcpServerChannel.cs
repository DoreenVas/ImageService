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
        private List<TcpClient> tcpClients;
        private Mutex mutexLock = new Mutex();

        public TcpServerChannel(int port, ILoggingService loggingService, IClientHandler clientHandler)
        {
            this.port = port;
            this.clientHandler = clientHandler;
            tcpClients = new List<TcpClient>();
            this.loggingService = loggingService;
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
                        mutexLock.WaitOne();
                        tcpClients.Add(client);
                        mutexLock.ReleaseMutex();
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
            //m_tcpClients.Clear();
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
                            mutexLock.WaitOne();
                            writer.Write(jsonCommand);
                            mutexLock.ReleaseMutex();
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
