using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public interface ICommunicationChannel
    {
        //event EventHandler<CommandRecievedEventArgs> CommandRecievedEventArgs;
        void Start();
        void Stop();
    }
}
