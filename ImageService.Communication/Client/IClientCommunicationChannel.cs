using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Client
{
    public delegate void CommandRecievedFromServer(CommandRecievedEventArgs commandRead);

    public interface IClientCommunicationChannel
    {
        event CommandRecievedFromServer ServerCommandRecieved;
        void Send(CommandRecievedEventArgs commandRecievedEventArgs);
    }
}
