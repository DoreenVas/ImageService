using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Communication.Server
{
    /// <summary>
    /// Represents a client handler.
    /// </summary>
    public interface IClientHandler
    {
        void HandleClient(TcpClient client, List<TcpClient> a_clients);
        object getLock();
    }
}
