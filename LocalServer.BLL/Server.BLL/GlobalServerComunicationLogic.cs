using LocalSerevr.DAL;
using LocalServer.BLL.DataManipulation.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;

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

            ClientToServerComunication("{ " + $" \"UserName\": \"{userName}\", \"Password\": \"{password}\" " + " }");
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
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data.Contains('|') ? data.Split('|')[1] : data);
            if (jObject.ContainsKey("Error"))
                throw new Exception(jObject["Error"].ToString());

            return jObject;
        }

        enum OperationTypes
        {
            GetDevices = 1,
            GetData = 2,
            SendData = 3,
            GetCount = 4,
            Authenticate = 5,
        }

        public static async Task AwaitServerCall()
        {
            string responseBufferNumber = String.Empty;
            NetworkStream stream = null;
            try
            {
                if (_tcpClient.Connected)
                {
                    stream = _tcpClient.GetStream();

                    while (_tcpClient.Connected)
                    {
                        byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                        int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (read > 0)
                        {
                            _data = buffer.SubArray(0, read);
                            string data = FormatData();
                            responseBufferNumber = data.Split('|')[0];
                            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data.Split('|')[1]);
                            JsonObject arguments = null;
                            OperationTypes operation = (OperationTypes)Enum.Parse(typeof(OperationTypes), jObject["OperationType"].ToString(), true);

                            switch (operation)
                            {
                                case OperationTypes.GetDevices:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|{GetDevices(int.Parse(arguments["UserId"].ToString()))}"));
                                    break;
                                case OperationTypes.GetData:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|" +
                                        $"{GetData(arguments["DeviceName"].ToString(), int.Parse(arguments["PagingSize"].ToString()), int.Parse(arguments["SkipAmount"].ToString()))}"));
                                    break;
                                case OperationTypes.SendData:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    SendData(arguments["DeviceName"].ToString(), arguments["Data"].ToString());
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|" + "[{\"Message\":\"Data Sent\"}]"));
                                    break;
                                case OperationTypes.GetCount:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|" + GetCount(arguments["DeviceName"].ToString())));
                                    break;
                                case OperationTypes.Authenticate:
                                    arguments = jObject["Arguments"] as JsonObject;
                                    stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|" + Authenticate(arguments["UserName"].ToString(), arguments["Password"].ToString())));
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
                stream.Write(Encoding.ASCII.GetBytes($"{responseBufferNumber}|Could not retrieve the data, {ex.Message}"));
            }
        }

        private static string GetDevices(int userId)
        {
            List<string> deviceNames = new List<string>();

            int roleId = int.Parse(DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users").First()
                .Select("UserId", "=", userId.ToString()).Rows[0]["RoleId"].ToString());

            DataTable permissions = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First()
                .Select("RoleId", "=", roleId.ToString());
            for (int i = 0; i < permissions.Rows.Count; i++)
            {
                if (bool.Parse(permissions.Rows[i]["CanRead"].ToString()))
                {
                    int deviceId = int.Parse(permissions.Rows[i]["DeviceId"].ToString());
                    DataTable device = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                        .Select("DeviceId", "=", deviceId.ToString());

                    if (bool.Parse(device.Rows[0]["IsAprooved"].ToString()))
                        deviceNames.Add(device.Rows[0]["Name"].ToString());
                }
            }
            List<object> serializableData = new List<object>();
            foreach (string name in deviceNames)
            {
                serializableData.Add(new { Name = name });
            }

            return JsonSerializer.Serialize(serializableData);
        }

        private static string GetData(string deviceName, int pagingSize, int skipAmount)
        {
            DataTable table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == deviceName).First()
                .Select("", "", "", pagingSize, skipAmount);

            table.Columns.Remove("Id");
            table.Columns.Remove("DeviceId");
            table.Columns["Created"].SetOrdinal(0);

            table.DefaultView.Sort = "Created desc";
            table = table.DefaultView.ToTable();

            return Table.ConvertDataTabletoString(table);
        }

        public static void SendData(string deviceName, string data)
        {
            string ipAddress = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Select("Name", "=", deviceName).Rows[0]["IPv4Address"].ToString();
            ServerLogic.GetClient(ipAddress).GetStream().Write(Encoding.ASCII.GetBytes(data));
        }

        private static string GetCount(string deviceName)
        {
            List<object> count = new List<object>
            {
                new { Count = DatabaseInitialiser.Database.Tables.Where(table => table.Name == deviceName).First().GetRowsCount() }
            };
            return JsonSerializer.Serialize(count);
        }

        private static string Authenticate(string userName, string password)
        {
            List<object> user = new List<object>
            {
                new { UserId = UserAuthenticationLogic.LogIn(userName, password) }
            };
            return JsonSerializer.Serialize(user);
        }

        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
    }
}
