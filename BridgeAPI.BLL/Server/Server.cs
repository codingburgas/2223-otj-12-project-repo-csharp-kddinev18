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
        private IAuthenticationService _authentication;

        private TcpListener _tcpListener;
        private List<TcpClient> _clients;
        private Dictionary<string, bool> _clientsIP;
        private Dictionary<Guid, string> _responseBuffer;
        private byte[] _data;
        private int _port;
        public Server(IAuthenticationService authentication)
        {
            _port = 5401;
            _data = new byte[16777216];
            _clientsIP = new Dictionary<string, bool>();
            _clients = new List<TcpClient>();
            _authentication = authentication;
            _responseBuffer = new Dictionary<Guid, string>();
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

        public string GetClientIP(TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }

        public void AcceptClients(IAsyncResult asyncResult)
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

        public async void ReciveClientInput(IAsyncResult asyncResult)
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
            catch (ArgumentNullException ex)
            {
                string response = "{\"Status\" : \"Failed\", \"ErrorMessage\":\"" + ex.Message + "\"}";
                client.Client.Send(Encoding.ASCII.GetBytes(response));
                DisconnectClient(client);
            }
            catch (Exception ex)
            {
                string response = "{\"Status\" : \"Failed\", \"ErrorMessage\":\"" + ex.Message + "\"}";
                client.Client.Send(Encoding.ASCII.GetBytes(response));
            }
            finally
            {
                FlushBuffer();
            }
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
        }

        public void FlushBuffer()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public void DisconnectClient(TcpClient client)
        {
            Console.WriteLine("Client disconnected");
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
            _clients.Remove(client);
            _clientsIP.Remove(GetClientIP(client));
            client = null;
        }

        public TcpClient GetClient(string clientIP)
        {
            return _clients.Where(client => GetClientIP(client) == clientIP).FirstOrDefault();
        }

        public string LocalServerCommunication(string message)
        {
            JsonObject jObject;
            TcpClient client;
            try
            {
                jObject = JsonSerializer.Deserialize<JsonObject>(message);
                client = GetClient(jObject["ClientIP"].ToString());
            }
            catch (Exception)
            {
                throw new Exception("JSON request is in incorrect format");
            }
            Guid guid = Guid.NewGuid();
            client.Client.Send(Encoding.ASCII.GetBytes("{\"RequestId\": \"" + guid + "\", {" + message + "}}"));

            _responseBuffer.Add(guid, string.Empty);

            int iterations = 0;
            while (string.IsNullOrEmpty(_responseBuffer[guid])) 
            {
                if(iterations >= 50) 
                {
                    throw new Exception("Request timeout");
                }
                Thread.Sleep(500);
                iterations++;
            }

            return _responseBuffer[guid];
        }
    }
}
