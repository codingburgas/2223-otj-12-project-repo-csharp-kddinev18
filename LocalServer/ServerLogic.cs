using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Xml.Linq;

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

        private static SqlConnection _sqlConnection;
        private string _connectionSting = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True";

        public ServerLogic(int port)
        {
            _port = port;
        }
        private void GetTableNames()
        {
            string query = @"SELECT name
                             FROM sys.tables";

            // A using in which we will execute our query
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                // A reader which will store the result of the query
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _clientsTables.Add(reader[0].ToString(), null);
                    }
                }
            }
        }

        public void ServerSetUp()
        {
            try
            {
                _sqlConnection = new SqlConnection(_connectionSting);
                _sqlConnection.Open();
                GetTableNames();

                _tcpListener = new TcpListener(IPAddress.Any, _port);
                // Starts the server
                _tcpListener.Start();
                // Starts accepting clients
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ServerShutDown();
            }
        }

        public void ServerShutDown()
        {
            _sqlConnection.Close();
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
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
        }

        public static void ReciveClientInput(IAsyncResult asyncResult)
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
                HandleClientInput(client, data);
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

        private static void HandleClientInput(TcpClient client, string data)
        {
            JsonObject JObject = JsonSerializer.Deserialize<JsonObject>(data);
            if (!JObject.ContainsKey("TableName"))
                throw new Exception("Invalid JSON format");

            if (!JObject.ContainsKey("OperationType"))
                throw new Exception("Invalid JSON format");

            if (_clientsTables.ContainsKey(JObject["TableName"].ToString()))
            {
                if (_clientsTables[JObject["TableName"].ToString()] == null)
                    _clientsTables[JObject["TableName"].ToString()] = client;
                else
                    throw new Exception("Can't have more than one client per table");
            }
            if (JObject["OperationType"].ToString() == "Create")
            {
                string columns = String.Empty;
                foreach (var keyValuePair in JObject)
                {
                    if (keyValuePair.Key == "TableName" || keyValuePair.Key == "OperationType")
                        continue;
                    columns += $"[{keyValuePair.Key}] {keyValuePair.Value} NOT NULL,\n";
                }
                string query =
                $@"CREATE TABLE {JObject["TableName"]}
                (
                    ID int IDENTITY(1,1) PRIMARY KEY NOT NULL,
                    {columns}
                );";
                using(SqlCommand cmd = new SqlCommand(query, _sqlConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            else if(JObject["OperationType"].ToString() == "Insert")
            {
                string columns = String.Empty;
                foreach (var keyValuePair in JObject)
                {
                    if (keyValuePair.Key == "TableName" || keyValuePair.Key == "OperationType")
                        continue;
                    columns += $"\'{keyValuePair.Value}\',";
                }
                columns = columns.Remove(columns.Length - 1, 1);
                string query =
                $@"INSERT INTO {JObject["TableName"]}
                VALUES (
                    {columns}
                );";
                using (SqlCommand cmd = new SqlCommand(query, _sqlConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
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
