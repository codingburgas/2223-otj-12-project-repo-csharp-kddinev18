﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocalServer
{
    public class ServerLogic
    {
        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static Dictionary<string, TcpClient> _clientsTables = new Dictionary<string, TcpClient>();
        // Buffer
        private static byte[] _data = new byte[16777216];
        private int _port;
        private static int _success = 0;
        private static int _error = 1;

        public ServerLogic(int port)
        {
            _port = port;
        }
        public void ServerSetUp()
        {
            _tcpListener = new TcpListener(IPAddress.Any, _port);
            // Starts the server
            _tcpListener.Start();
            // Starts accepting clients
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
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
            TcpClient client;
            try
            {
                // Connect the client
                client = _tcpListener.EndAcceptTcpClient(asyncResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            // Add the client newly connect client into the _clients list
            _clients.Add(client);
            // Begin recieving bytes from the client
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveUserInput), client);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
        }

        public static void ReciveUserInput(IAsyncResult asyncResult)
        {
            TcpClient client = asyncResult.AsyncState as TcpClient;
            int reciever;
            List<string> args;
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
                string data = Encoding.ASCII.GetString(_data).Replace("\0", String.Empty);
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
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveUserInput), client);
        }

        // Clear the buffer
        public static void FlushBuffer()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public static void DisconnectClient(TcpClient client)
        {
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
            //_dbContexts.Remove(client);
            _clients.Remove(client);
        }
    }
}