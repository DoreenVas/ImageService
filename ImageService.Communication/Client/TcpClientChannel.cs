 using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
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
        private TcpClient client;
        private bool connected = false;
        private object obj = new object();
        public bool IsConnected { get; private set; }

        public delegate void CommandRecievedFromServer(CommandRecievedEventArgs commandRead);
        public event Client.CommandRecievedFromServer ServerCommandRecieved;

        public static TcpClientChannel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TcpClientChannel();
                }
                return instance;
            }
        }

        private TcpClientChannel() 

        {
            if (!connected)
            {
                Start();
                IsConnected = connected;
            }
        }

        private void Start()
        {
            try
            {

                client = new TcpClient();
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
                client.Connect(ipEndPoint);
                //Console.WriteLine("You are connected");
                connected = true;

            }
            catch (Exception exception)
            {
                connected = false;
                //Console.WriteLine(exception.ToString());
            }
        }

        public void SendCommand(CommandRecievedEventArgs commandRecievedEventArgs)
        {
            new Task(() =>
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    string strJsonCmd = JsonConvert.SerializeObject(commandRecievedEventArgs);
                    //Console.WriteLine("Send to server:" + JsonConvert.SerializeObject(commandRecievedEventArgs, Newtonsoft.Json.Formatting.Indented));
                    lock(obj)
                    {
                        writer.Write(strJsonCmd);
                    }
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception.ToString());
                }
            }).Start();
        }

        public void RecieveCommand()
        {
            new Task(() =>
            {
                try
                {
                    while (connected)
                    {
                        NetworkStream stream = client.GetStream();
                        BinaryReader reader = new BinaryReader(stream);
                        string commandRead = reader.ReadString();
                        //Console.WriteLine("Recieved from server:\n" + commandRead);
                        CommandRecievedEventArgs command = JsonConvert.DeserializeObject<CommandRecievedEventArgs>(commandRead);
                        ServerCommandRecieved?.Invoke(command);
                    }
                }
                catch (Exception exception)
                {
                    //Console.WriteLine(exception.ToString());
                }
            }).Start();
        }

        public void CloseClient()
        {
            connected = false;
            CommandRecievedEventArgs command = new CommandRecievedEventArgs((int)CommandEnum.ClientClosedCommand, null, "bla");
            SendCommand(command);
        }
        
    }
}
