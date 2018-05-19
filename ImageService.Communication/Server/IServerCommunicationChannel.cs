using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication.Server
{
    public interface IServerCommunicationChannel 
    {
        //event EventHandler<CommandRecievedEventArgs> CommandRecievedEventArgs;
        void Start();
        void Stop();
        void NotifyClients(CommandRecievedEventArgs commandRecievedEventArgs);
    }
}
