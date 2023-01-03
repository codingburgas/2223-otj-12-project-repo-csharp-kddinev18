using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Diagnostics;
using DataAccessLayer;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections.Generic;
using System.Timers;
using System;
using System.Data.SqlClient;
using System.Net.Http;

namespace LocalServerLogic
{
    public class ServerLogic
    {
        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurity;Integrated Security=True;MultipleActiveResultSets=true";

        // Buffer
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
                BusinessLogic.InitialiseDatabase(_connectionString, deleteTimer);

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
            _tcpListener.Stop();
            _tcpListener = null;
        }

        public static void AcceptClients(IAsyncResult asyncResult)
        {
            // Newly connection client
            TcpClient client = null;
            bool accepted = false;
            try
            {
                // Connect the client
                client = _tcpListener.EndAcceptTcpClient(asyncResult);
                Console.WriteLine("Client connected with IP {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                accepted = BusinessLogic.AddClients(client);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (accepted)
            {
                // Add the client newly connect client into the _clients list
                _clients.Add(client);
                // Begin recieving bytes from the client
                client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
            }
            else
            {
                DisconnectClient(client);
            }
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
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
                BusinessLogic.HandleClientInput(Encoding.ASCII.GetString(_data).Replace("\0", String.Empty), _clients);
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
            _clients.Remove(client);
            client = null;
        }

        public void AprooveClient(string ipAddress)
        {
            BusinessLogic.AprooveClient(ipAddress);
        }
    }
}