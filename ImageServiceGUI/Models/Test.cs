using ImageService.Communication.Client;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    class Test
    {
        private IClientCommunicationChannel client;

        public Test()
        {
            client = new TcpClientChannel();
            int commandId = (int)CommandEnum.NewFileCommand;
            string[] args = {  };
            CommandRecievedEventArgs commandArgs = new CommandRecievedEventArgs(commandId, args, "");
            client.Send(commandArgs);
        }
    }
}
