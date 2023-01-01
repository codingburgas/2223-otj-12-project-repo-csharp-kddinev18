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

namespace LocalServerLogic
{
    public class ServerLogic
    {
        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static Database _database;
        private static string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ORMTest;Integrated Security=True;MultipleActiveResultSets=true";

        // Buffer
        private static byte[] _data = new byte[16777216];

        private int _port;
        private long _deleteTimer;
        private static int _success = 0;
        private static int _error = 1;

        public ServerLogic(int port)
        {
            _port = port;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (Table table in Database.Tables)
            {
                if (table.Name == "Users" || table.Name == "Devices" || table.Name == "Permissions")
                    continue;

                string query = $"DELETE FROM [{table.Name}] WHERE [Created] < DATEADD(mi,{(int)_deleteTimer/(1000 * 60)},GETDATE())";
                using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ServerSetUp(long deleteTimer)
        {
            try
            {
                _database = new Database(_connectionString);
                BusinessLogic.CreateDefaultDatabaseStructure(_database);

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
            try
            {
                // Connect the client
                client = _tcpListener.EndAcceptTcpClient(asyncResult);
                string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();


                string devicesJson = Table.ConvertDataTabletoString(Database.Tables.Where(table => table.Name == "Devices").First().Select("IPv4Address", "=", clientIpAddress));
                List<JsonObject> devices = JsonSerializer.Deserialize<List<JsonObject>>(devicesJson);
                if (devices.Count == 0)
                {
                    Database.Tables.Where(table => table.Name == "Devices").First().Insert(clientIpAddress, clientIpAddress, "false");
                    _database.SaveDatabaseData();
                    DisconnectClient(client);
                    client = null;
                    throw new Exception("Client not accepted");
                }
                else
                {
                    if (bool.Parse(devices.First()["IsAprooved"].ToString()) == false)
                    {
                        DisconnectClient(client);
                        client = null;
                        throw new Exception("Client is banned");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (client is not null)
            {
                // Add the client newly connect client into the _clients list
                _clients.Add(client);
                // Begin recieving bytes from the client
                client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
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
                HandleClientInput(Encoding.ASCII.GetString(_data).Replace("\0", String.Empty));
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

        private static void HandleClientInput(string data)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
            if (jObject["Type"].ToString() == "Authenticate")
            {
                if (!Database.Tables.Select(table => table.Name).Contains(jObject["Name"].ToString()))
                {
                    Table newTable = new Table(jObject["Name"].ToString());
                    foreach (JsonObject column in JsonSerializer.Deserialize<List<JsonObject>>(jObject["Columns"].ToString()))
                    {
                        Column newColumn = new Column(column["Name"].ToString(), column["Type"].ToString(), newTable);
                        foreach (JsonObject constraint in JsonSerializer.Deserialize<List<JsonObject>>(column["Constraints"].ToString()))
                        {
                            if (constraint["Constraint"].ToString() == "FOREIGN KEY")
                            {
                                string tableName = constraint["AdditionalInformation"].ToString().Split(", ")[0];
                                string columnsName = constraint["AdditionalInformation"].ToString().Split(", ")[1];
                                Column foreignKeyColumn = Database.Tables.Where(table => table.Name == tableName).First()
                                    .Columns.Where(column => column.Name == columnsName).First();
                                newColumn.AddConstraint(new Tuple<string, object>(constraint["Constraint"].ToString(), foreignKeyColumn));
                            }
                            else
                            {
                                newColumn.AddConstraint(new Tuple<string, object>(constraint["Constraint"].ToString(), constraint["AdditionalInformation"].ToString()));
                            }
                        }
                        newTable.Columns.Add(newColumn);
                    }
                    Column systemColumn = new Column("Created", "datetime2(7)", newTable);
                    systemColumn.AddConstraint(new Tuple<string, object>("DEFAULT", "GETDATE()"));
                    newTable.Columns.Add(systemColumn);

                    Database.Tables.Add(newTable);
                    _database.SaveDatabaseInfrastructure();
                }
            }
            else if (jObject["Type"].ToString() == "Insert")
            {
                Table table = Database.Tables.Where(table => table.Name == jObject["Name"].ToString()).First();
                List<string> insertData = new List<string>();

                table.Insert(JsonSerializer.Deserialize<List<string>>(jObject["Columns"].ToString()).ToArray());
                _database.SaveDatabaseData();
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
            _clients.Remove(client);
        }
    }
}