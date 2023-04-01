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
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DTO.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeAPI.BLL
{
    public class Server : IServer
    {
        private IServiceProvider _serviceProvider;

        private TcpListener _tcpListener;
        private static Dictionary<TcpClient, Token> _clients;
        private static Dictionary<Guid, string> _responseBuffer;
        private byte[] _data;
        private int _port;
        public Server(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _port = 5401;
            _data = new byte[16777216];
            _clients = new Dictionary<TcpClient, Token>();
            _responseBuffer = new Dictionary<Guid, string>();
        }
        public void ServerSetUp(int port = 5401)
        {
            try
            {
                _port = port;
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
            foreach (KeyValuePair<TcpClient, Token> client in _clients)
            {
                DisconnectClient(client.Key);
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
            _clients.Add(client, null);
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
                string data = Encoding.UTF8.GetString(_data).Replace("\0", string.Empty);
                if(!await AuthenticateClient(client, data))
                    GetResponse(data);
            }
            catch (ArgumentException ex)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IResponseFormatterService responseFormatterService = scope.ServiceProvider.GetRequiredService<IResponseFormatterService>();

                string response = responseFormatterService.FormatResponseToString(400,ex.Message,ex.Message, null);
                client.Client.Send(Encoding.UTF8.GetBytes(response));
                DisconnectClient(client);
            }
            catch (Exception ex)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IResponseFormatterService responseFormatterService = scope.ServiceProvider.GetRequiredService<IResponseFormatterService>();
                
                string response = responseFormatterService.FormatResponseToString(400, "General Error", ex.Message, null);
                client.Client.Send(Encoding.UTF8.GetBytes(response));
            }
            finally
            {
                FlushBuffer();
            }
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
        }

        private void GetResponse(string data)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
            Guid responseId = new Guid(jObject["ResponseId"].ToString());
            if (_responseBuffer.ContainsKey(responseId))
            {
                _responseBuffer[responseId] = JsonSerializer.Serialize(jObject);
            }
            else
            {
                throw new ArgumentException("Wrong response");
            }
        }

        private async Task<bool> AuthenticateClient(TcpClient client, string data)
        {
            if (_clients[client] == null)
            {
                using IServiceScope scope = _serviceProvider.CreateScope();
                IAuthenticationService authenticationService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
                ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
                IResponseFormatterService responseFormatterService = scope.ServiceProvider.GetRequiredService<IResponseFormatterService>();

                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
                IResponseDataTransferObject user = await authenticationService.LogInAsync(new UserRequestDataTransferObject()
                {
                    UserName = jObject["UserName"].ToString(),
                    Password = jObject["Password"].ToString()
                });
                Token token = _clients[client] = await tokenService.GenerateTokenType(user);
                string response = responseFormatterService.FormatResponseToString(200, JsonSerializer.Serialize(token), null, null);
                client.Client.Send(Encoding.UTF8.GetBytes(response));

                return true;
            }
            return false;
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
            client = null;
        }

        public TcpClient GetClient(string clientIP)
        {
            return _clients.Where(client => GetClientIP(client.Key) == clientIP).FirstOrDefault().Key;
        }

        public static TcpClient GetClient(Guid tokenId)
        {
            return _clients.Where(client => client.Value.TokenId == tokenId).FirstOrDefault().Key;
        }

        public static async Task<string> LocalServerCommunication(string arguments)
        {
            JsonObject jObject;
            TcpClient client;
            try
            {
                jObject = JsonSerializer.Deserialize<JsonObject>(arguments);
                client = GetClient(new Guid(jObject["TokenId"].ToString()));
                jObject.Remove("TokenId");
            }
            catch (Exception)
            {
                throw new Exception("JSON request is in incorrect format");
            }
            Guid guid = Guid.NewGuid();
            jObject.Add("RequestId", guid);
            client.Client.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(jObject)));

            _responseBuffer.Add(guid, string.Empty);

            int iterations = 0;
            while (string.IsNullOrEmpty(_responseBuffer[guid])) 
            {
                if(iterations >= 50) 
                {
                    throw new TimeoutException("Request timeout");
                }
                await Task.Delay(500);
                iterations++;
            }

            string response = _responseBuffer[guid];
            _responseBuffer.Remove(guid);

            return response;
        }
    }
}
