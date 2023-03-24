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
        public void ServerSetUp();
        public void ServerShutDown();
        public TcpClient GetClient(string clientIP);
        public string LocalServerCommunication(string message);
    }
}
