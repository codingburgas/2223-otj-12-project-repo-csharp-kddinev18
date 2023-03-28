using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.DAL;
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
    public static class BridgeAPIHandlingLogic
    {
        private static byte[] _data = new byte[16777216];
        private static TcpClient _tcpClient;
        private static Dictionary<string, Func<string, string>> _operations = new Dictionary<string, Func<string, string>>()
        {
            { "Authenticate", Authenticate },
            { "GetRowsCount", GetRowsCount },
            { "GetDevices", GetDevices },
            { "GetDataFromDevice", GetDataFromDevice },
            { "SendDataToDevice", SendDataToDevice },
        };
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
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
            if (int.Parse(jObject["StatusCode"].ToString()) / 100 != 2)
                throw new Exception(jObject["Error"].ToString());

            return jObject;
        }

        public static async Task AwaitServerCall()
        {
            string responseBufferNumber = String.Empty;
            NetworkStream stream = null;
            string response = string.Empty;
            if (_tcpClient.Connected)
            {
                stream = _tcpClient.GetStream();

                while (_tcpClient.Connected)
                {
                    byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                    int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read > 0)
                    {
                        try
                        {
                            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>
                                (Encoding.ASCII.GetString(buffer).Replace("\0", string.Empty));

                            response = FormatResponse(
                                200,
                                _operations[jObject["OperationType"].ToString()].Invoke(jObject["Arguments"].ToString()),
                                null,
                                null
                            );
                        }
                        catch (JsonException)
                        {
                            response = FormatResponse(400, "Incorrect request", "Incorrect request", null);
                        }
                        catch (NullReferenceException)
                        {
                            response = FormatResponse(400, "Incorrect request", "Incorrect request", null);
                        }
                        catch (Exception)
                        {
                            response = FormatResponse(500, "Server error", "Server error", null);
                        }
                        finally
                        {
                            stream.Write(Encoding.ASCII.GetBytes(response));
                        }
                    }
                }
            }
        }

        public static string FormatResponse(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation)
        {
            return JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Response = response,
                errorMessage = errorMessage ?? null,
                AdditionalInformation = additionalInformation
            },
            new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
        }

        private static string GetDevices(string parameters)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(parameters);
            List<string> deviceNames = new List<string>();

            int roleId = int.Parse(DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users").First()
                .Select("UserId", "=", jObject["UserId"].ToString()).Rows[0]["RoleId"].ToString());

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

        private static string GetDataFromDevice(string parameters)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(parameters);

            DataTable table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == jObject["DeviceName"].ToString()).First()
                .Select("", "", "", int.Parse(jObject["PagingSize"].ToString()), int.Parse(jObject["SkipAmount"].ToString()));

            table.Columns.Remove("Id");
            table.Columns.Remove("DeviceId");
            table.Columns["Created"].SetOrdinal(0);

            table.DefaultView.Sort = "Created desc";
            table = table.DefaultView.ToTable();

            return Table.ConvertDataTabletoString(table);
        }

        public static string SendDataToDevice(string parameters)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(parameters);

            string ipAddress = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Select("Name", "=", jObject["DeviceName"].ToString()).Rows[0]["IPv4Address"].ToString();
            ServerLogic.GetClient(ipAddress).GetStream().Write(Encoding.ASCII.GetBytes(jObject["Data"].ToString()));
        }

        private static string GetRowsCount(string parameters)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(parameters);
            List<object> count = new List<object>
            {
                new { Count = DatabaseInitialiser.Database.Tables.Where(table => table.Name == jObject["DeviceName"].ToString()).First().GetRowsCount() }
            };
            return JsonSerializer.Serialize(count);
        }

        private static string Authenticate(string parameters)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(parameters);

            List<object> user = new List<object>
            {
                new { UserId = UserAuthenticationLogic.LogIn(jObject["UserName"].ToString(), jObject["Password"].ToString()) }
            };
            return JsonSerializer.Serialize(user);
        }
    }
}
