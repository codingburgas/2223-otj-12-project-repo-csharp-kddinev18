using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace LocalServer.BLL.Server.BLL
{
    public static class GlobalServerComunicationLogic
    {
        private static byte[] _data = new byte[16777216];
        private static TcpClient _tcpClient;

        // Connect to the server
        public static void SetUpConnection(string userName, string password)
        {
            if (_tcpClient == null)
                _tcpClient = new TcpClient("127.0.0.1", 5400);

            ClientToServerComunication("{ "+$"UserName: \"{userName}\", Password: \"{password}\" "+" }");
        }

        // Disconenct from the servet
        public static void RemoveConnection()
        {
            if (_tcpClient != null)
            {
                _tcpClient.Client.Shutdown(SocketShutdown.Both);
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        // Convert the bytes into a string
        public static string FormatData()
        {
            return Encoding.ASCII.GetString(_data).Replace("\0", string.Empty);
        }

        // Clear the data buffer
        public static void FlushBuffer()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        // Communication with the server
        private static JsonObject ClientToServerComunication(string message)
        {
            // Clear the data buffer
            FlushBuffer();

            // Send message to the server
            _tcpClient.Client.Send(Encoding.UTF8.GetBytes(message));
            // Wait until a response is recieved
            _tcpClient.Client.Receive(_data);

            string data = FormatData();
            // Format tha data
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
            if (jObject.ContainsKey("Error"))
                throw new Exception(jObject["Error"].ToString());

            return jObject;
        }

        public static async Task AwaitServerCall()
        {
            try
            {
                //if (_tcpClient.Connected)
                //{
                    //NetworkStream stream = _tcpClient.GetStream();

                    while (true)//_tcpClient.Connected)
                    {
                        //byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                        //int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        //if (read > 0)
                        //{
                            // you have received a message, do something with it
                        //}
                    }
                //}
            }
            catch (Exception ex)
            {
                // display the error message or whatever
                _tcpClient.Client.Shutdown(SocketShutdown.Both);
                _tcpClient.Close();
                _tcpClient = null;
            }
        }
    }
}
