using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Server
{
    public class TcpServerChannel : IServerCommunicationChannel
    {
        private int port;
        private TcpListener tcpListener;
        //private ILoggingService m_loggingService;
        private List<TcpClient> tcpClients;
        private IClientHandler clientHandler;
        //private bool m_connected; // todo: check if needed
        //private Mutex m_mutexLock = new Mutex();

        //public event EventHandler<CommandRecievedEventArgs> CommandRecieved; //todo: modify using place

        public TcpServerChannel(int port, /*ILoggingService a_loggingService,*/ IClientHandler clientHandler)
        {
            this.port = port;
            this.clientHandler = clientHandler;
            tcpClients = new List<TcpClient>();
            //m_loggingService = a_loggingService;
            //m_connected = false; // todo: check if needed
        }

        public void Start()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            tcpListener = new TcpListener(ipEndPoint);
            tcpListener.Start();
            //m_loggingService.Log("Created a new TCP server channel.", MessageTypeEnum.INFO); //Todo: change to LoggingServiceMessageTypeEnum.INFO
            //m_loggingService.Log("Waiting for connections...", MessageTypeEnum.INFO); //Todo: change to LoggingServiceMessageTypeEnum.INFO
            //m_connected = true;
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        //m_loggingService.Log("Established connection with: " + client.ToString(), Logging.Modal.MessageTypeEnum.INFO); //Todo: change to LoggingServiceMessageTypeEnum.INFO
                        //m_mutexLock.WaitOne();
                        tcpClients.Add(client);
                        //m_mutexLock.ReleaseMutex();
                        clientHandler.HandleClient(client, tcpClients);
                    }
                    catch (SocketException)
                    {
                        //m_connected = false; // todo: check if needed
                        //m_loggingService.Log(socketException.ToString(), Logging.Modal.MessageTypeEnum.FAIL); //Todo: change to LoggingServiceMessageTypeEnum.ERROR
                        break;
                    }
                }
                //m_loggingService.Log("Server stopped.", Logging.Modal.MessageTypeEnum.INFO); //Todo: change to LoggingServiceMessageTypeEnum.INFO
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
            //m_connected = false;
        }

        /*
        private void NotifyClients(CommandRecievedEventArgs commandRecievedEventArgs)
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
                            //m_mutexLock.WaitOne();
                            writer.Write(jsonCommand);
                            //m_mutexLock.ReleaseMutex();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.ToString());
                            tcpClients.Remove(tcpClient);
                        }
                    }).Start();
                }
            }
            catch (Exception exception)
            {
                //m_loggingService.Log(exception.ToString(), MessageTypeEnum.FAIL);
            }
        }
        */

    }
}
