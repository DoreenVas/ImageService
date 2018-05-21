using ImageService.Communication.Client;
using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpClientChannel clientCh = TcpClientChannel.Instance;
            string[] bla = { "bla", "bla" };
            CommandRecievedEventArgs command = new CommandRecievedEventArgs(4, bla, "bli");
            clientCh.SendCommand(command);
            Console.Read();
        }
    }
}
