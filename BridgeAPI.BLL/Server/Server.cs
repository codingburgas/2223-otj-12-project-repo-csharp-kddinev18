using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DAL.Repositories;
using BridgeAPI.DAL.Repositories.Interfaces;
using System.Text.Json.Nodes;
using System.Text.Json;
using BridgeAPI.DTO;

namespace BridgeAPI.BLL
{
    public class Server : IServer
    {
        private static IAuthenticationService _authentication;

        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static Dictionary<string, bool> _clientsIP = new Dictionary<string, bool>();
        private static byte[] _data = new byte[16777216];
        private int _port;
        public Server(IAuthenticationService authentication)
        {
            _port = 5401;
            _authentication = authentication;
        }
        public void ServerSetUp()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, _port);
                _tcpListener.Start();
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ServerShutDown();
            }
        }

        public void ServerShutDown()
        {
            if (_tcpListener != null)
                _tcpListener.Stop();
            foreach (TcpClient client in _clients)
            {
                DisconnectClient(client);
            }
            _tcpListener = null;
        }

        public static string GetClientIP(TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }

        public static void AcceptClients(IAsyncResult asyncResult)
        {
            TcpClient client = null;
            try
            {
                client = _tcpListener.EndAcceptTcpClient(asyncResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _clients.Add(client);
            _clientsIP.Add(GetClientIP(client), false);
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            if (_tcpListener is not null)
            {
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            }
        }

        public async static void ReciveClientInput(IAsyncResult asyncResult)
        {
            TcpClient client = asyncResult.AsyncState as TcpClient;
            int reciever;
            try
            {
                reciever = client.Client.EndReceive(asyncResult);
                if (reciever == 0)
                {
                    DisconnectClient(client);
                    return;
                }
                string data = Encoding.ASCII.GetString(_data).Replace("\0", string.Empty);
                if (_clientsIP[GetClientIP(client)] == false)
                {
                    JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
                    await _authentication.LogInAsync(new UserRequestDataTrasferObject()
                    {
                        UserName = jObject["UserName"].ToString(),
                        Password = jObject["Password"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                string response = $"{_error}|{ex.Message}";
                client.Client.Send(Encoding.ASCII.GetBytes(response));
            }
            finally
            {
                FlushBuffer();
            }
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
        }

        public static void FlushBuffer()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public static void DisconnectClient(TcpClient client)
        {
            Console.WriteLine("Client disconnected");
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
            _clients.Remove(client);
            client = null;
        }
    }
}
