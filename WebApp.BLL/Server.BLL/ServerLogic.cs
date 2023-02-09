using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections.Generic;
using System.Timers;
using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Net.Http.Headers;
using WebApp.DAL.Data;
using WebApp.DAL.Models;

namespace WebApp.BLL.Server.BLL
{
    public class ServerLogic
    {
        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static Dictionary<string, Tuple<int, bool>> _aproovedClients = new Dictionary<string, Tuple<int, bool>>();
        private static IOTHomeSecurityDbContext _dbContext = new IOTHomeSecurityDbContext();
        private static Dictionary<int, string> _resposeBuffer = new Dictionary<int, string>();
        private static int _responseBufferCount = 0;
        private static byte[] _data = new byte[16777216];

        private int _port;
        private static int _success = 0;
        private static int _error = 1;

        public ServerLogic(int port)
        {
            _port = port;
        }
        public void ServerSetUp(long deleteTimer)
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, _port);
                // Starts the server
                _tcpListener.Start();
                // Starts accepting clients
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
            // Stops the server
            if (_tcpListener != null)
                _tcpListener.Stop();
            foreach (TcpClient client in _clients)
            {
                DisconnectClient(client);
            }
            _tcpListener = null;
        }

        public static void AcceptClients(IAsyncResult asyncResult)
        {
            // Newly connection client
            TcpClient client = null;
            try
            {
                // Connect the client
                client = _tcpListener.EndAcceptTcpClient(asyncResult);
                _aproovedClients.Add(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), new Tuple<int, bool>(-1, false));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Add the client newly connect client into the _clients list
            _clients.Add(client);
            // Begin recieving bytes from the client
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            if (_tcpListener is not null)
            {
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            }
        }

        public static void ReciveClientInput(IAsyncResult asyncResult)
        {
            TcpClient client = asyncResult.AsyncState as TcpClient;
            int reciever;
            try
            {
                // How many bytes has the user sent
                reciever = client.Client.EndReceive(asyncResult);
                // If the bytes are - disconnect the client
                if (reciever == 0)
                {
                    DisconnectClient(client);
                    return;
                }
                // Get the data
                string data = Encoding.ASCII.GetString(_data).Replace("\0", string.Empty);
                if (AuthenticateClient(client, data))
                {
                    if (data.Contains('|'))
                    {
                        _resposeBuffer[int.Parse(data.Split('|')[0])] = data.Split('|')[1];
                    }
                }
                else
                {

                    DisconnectClient(client);
                    _aproovedClients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                }

            }
            catch (Exception ex)
            {
                string response = $"{_error}|{ex.Message}";
                // send data to the client
                client.Client.Send(Encoding.ASCII.GetBytes(response));
            }
            finally
            {
                FlushBuffer();
            }
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
        }

        private static bool AuthenticateClient(TcpClient client, string data)
        {
            string ipAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            if (_aproovedClients[ipAddress].Item2 == false)
            {
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
                User user = _dbContext.Users.Where(user => user.UserName == jObject["UserName"].ToString()).First();
                string hashedPassword = Hash(string.Concat(jObject["Password"].ToString(), user.Salt));
                if (hashedPassword == user.Password)
                {
                    _aproovedClients[ipAddress] = new Tuple<int, bool>(user.Id, true);

                    NetworkStream stream = client.GetStream();
                    string sendingData = "{}";
                    byte[] msg = Encoding.ASCII.GetBytes(sendingData);

                    //Send to Client
                    stream.Write(msg, 0, msg.Length);

                    return true;
                }
                else
                {
                    NetworkStream stream = client.GetStream();
                    string sendingData = "{\"Error\":\"Authentication not successful\"}";
                    byte[] msg = Encoding.ASCII.GetBytes(sendingData);

                    //Send to Client
                    stream.Write(msg, 0, msg.Length);

                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        // Clear the buffer
        public static void FlushBuffer()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public static void DisconnectClient(TcpClient client)
        {
            Console.WriteLine("Client disconnected");
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
            _aproovedClients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
            _clients.Remove(client);
            client = null;
        }

        public static IEnumerable<JsonObject> LocalServerCommunication(int userId, string message)
        {
            int container = ++_responseBufferCount;
            string clientIp = _aproovedClients.Where(aproovedClient => aproovedClient.Value.Item1 == userId).FirstOrDefault().Key;
            if (clientIp != null)
            {
                TcpClient client = _clients.Where(client => ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() == clientIp).First();
                NetworkStream stream = client.GetStream();
                byte[] msg = Encoding.ASCII.GetBytes($"{container}|{message}");

                //Send to Client
                stream.Write(msg, 0, msg.Length);

                _resposeBuffer.Add(container, string.Empty);
                while (_resposeBuffer[container] == string.Empty)
                {
                    Thread.Sleep(500);
                }
                return JsonSerializer.Deserialize<IEnumerable<JsonObject>>(_resposeBuffer[container]);
            }
            throw new Exception("There isn't a local server connected associated with that user");
        }

        private static string Hash(string data)
        {
            // Conver the output to a string and return it
            return BitConverter.ToString
                (
                    // Hashing
                    SHA256.Create().ComputeHash
                    (
                        // convert the string into bytes using UTF8 encoding
                        Encoding.UTF8.GetBytes(data)
                    )
                )
                // Convert all the characters in the string to a uppercase characters
                .ToUpper()
                // Remove the '-' from the hashed data
                .Replace("-", "");
        }
    }
}