using ImageService.Infrastructure;
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
using System.Xml;

namespace ImageService.Communication.Client
{
    public class TcpClientChannel : IClientCommunicationChannel
    {
        private static TcpClientChannel instance;
        private IPEndPoint ipEndPoint;
        private TcpClient client;
        private IClientCommunicationChannel handler;
        private bool connected;
        private Mutex mutexLock = new Mutex();

        //public delegate void CommandRecievedFromServer(CommandRecievedEventArgs commandRead);
        public event CommandRecievedFromServer ServerCommandRecieved;

        public TcpClientChannel() // todo private
        {
            if (!connected)
            {
                Start();
            }
        }

        private bool Start()
        {
            try
            {

                client = new TcpClient();
                IPEndPoint m_ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
                client.Connect(m_ipEndPoint);
                Console.WriteLine("You are connected");
                // todo: maybe send this to log
                connected = true;

            }
            catch (Exception exception)
            {
                connected = false;
                Console.WriteLine(exception.ToString());
            }
            return connected;
        }

        public void Send(CommandRecievedEventArgs commandRecievedEventArgs)
        {
            new Task(() =>
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    string strJsonCmd = JsonConvert.SerializeObject(commandRecievedEventArgs);
                    Console.WriteLine("Send to server:\n" + JsonConvert.SerializeObject(commandRecievedEventArgs, Newtonsoft.Json.Formatting.Indented)); //todo: check that it works.
                    mutexLock.WaitOne();
                    writer.Write(strJsonCmd);
                    // todo: maybe flush here ?
                    mutexLock.ReleaseMutex();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }).Start();
        }

        public void Recieve()
        {
            new Task(() =>
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryReader reader = new BinaryReader(stream);

                    while (connected)
                    {

                        string commandRead = reader.ReadString();
                        Console.WriteLine("Recieved from server:\n" + commandRead);
                        CommandRecievedEventArgs commandObj = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(commandRead);
                        ServerCommandRecieved?.Invoke(commandObj);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }).Start();
        }

        public void Close()
        {
            client.Close();
        }
    }
}
