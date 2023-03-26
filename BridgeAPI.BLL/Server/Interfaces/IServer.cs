using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Interfaces
{
    public interface IServer
    {
        public void ServerSetUp(int port = 5401);
        public void ServerShutDown();
        public TcpClient GetClient(string clientIP);
        public TcpClient GetClient(Guid tokenId);
        public Task<string> LocalServerCommunication(string message);
    }
}
