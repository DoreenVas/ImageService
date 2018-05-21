using ImageService.Communication.Client;
using ImageService.Communication.Server;
using ImageService.Controller;
using ImageService.Logging;
using ImageService.Modal;
using ImageService.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            IClientHandler clientHandler = new TcpClientHandler();
            ILoggingService log = new LoggingService();
            TcpServerChannel server = new TcpServerChannel(8000, log, clientHandler);
            server.Start();
            Console.Read();
        }
    }
}
