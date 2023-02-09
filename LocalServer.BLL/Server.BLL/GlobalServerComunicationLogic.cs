using LocalServer.BLL.DataManipulation.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }

        // Connect to the server
        public static void SetUpConnection(string userName, string password)
        {
            if (_tcpClient == null)
                _tcpClient = new TcpClient("127.0.0.1", 5401);

            ClientToServerComunication("{ "+$" \"UserName\": \"{userName}\", \"Password\": \"{password}\" " +" }");
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

        enum OperationTypes
        {
            GetDevices = 1,
            GetData = 2,
            SentData = 3
        }

        public static async Task AwaitServerCall()
        {
            try
            {
                if (_tcpClient.Connected)
                {
                    NetworkStream stream = _tcpClient.GetStream();

                    while (_tcpClient.Connected)
                    {
                        byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                        int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            _data = buffer.SubArray(0, read);
                            string data = FormatData();
                            string responseBufferNumber = data.Split('|')[0];
                            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data.Split('|')[1]);
                            JsonObject arguments = null;
                            OperationTypes operation = (OperationTypes)Enum.Parse(typeof(OperationTypes), jObject["OperationType"].ToString(), true);

                            switch (operation)
                            {
                                case OperationTypes.GetDevices:
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|{GetDevices()}"));
                                    break;
                                case OperationTypes.GetData:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|" +
                                        $"{GetData(arguments["IpAddress"].ToString(), int.Parse(arguments["PagingSize"].ToString()), int.Parse(arguments["SkipAmount"].ToString()))}"));
                                    break;
                                case OperationTypes.SentData:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    SendData(arguments["IpAddress"].ToString(), arguments["Data"].ToString());
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // display the error message or whatever
                _tcpClient.Client.Shutdown(SocketShutdown.Both);
                _tcpClient.Close();
                _tcpClient = null;
            }
        }

        class DeviceNameDTO
        {
            public string Name { get; set; }
        }

        private static string GetDevices()
        {
            IEnumerable<string> names = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name != "Users" || table.Name != "Devices" ||
                table.Name != "Roles" || table.Name != "Permissions" || table.Name != "sys.diagrams")
                .Select(table => table.Name);
            string test = JsonSerializer.Serialize(names);
            List<DeviceNameDTO> serializableData = new List<DeviceNameDTO>();
            foreach (string name in names)
            {
                serializableData.Add(new DeviceNameDTO() { Name = name });
            }

            return JsonSerializer.Serialize(serializableData);
        }

        private static string GetData(string ipAddress, int pagingSize, int skipAmount)
        {
            string deviceName = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Select("IPv4Address", "=", ipAddress).Rows[0]["Name"].ToString();

            return JsonSerializer.Serialize(DatabaseInitialiser.Database.Tables.Where(table => table.Name == deviceName).First()
                .Select("", "", "", pagingSize, skipAmount));
        }

        private static void SendData(string ipAddress, string data)
        {
            ServerLogic.GetClient(ipAddress).GetStream().Write(Encoding.ASCII.GetBytes(data));
        }

        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
}
