using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Diagnostics;
using DataAccessLayer;

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
        private static int _success = 0;
        private static int _error = 1;

        public ServerLogic(int port)
        {
            _port = port;
        }

        public void ServerSetUp()
        {
            try
            {
                CreateDefaultDatabaseStructure();

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

        private static void CreateDefaultDatabaseStructure()
        {
            _database = new Database(_connectionString);
            _database.LoadDatabaseInfrastructure();
            if (!Database.Tables.Select(table => table.Name).Contains("Devices"))
            {
                Table devicesTable = new Table("Devices");

                Column deviceId = new Column("DeviceId", "int", devicesTable);
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                deviceId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column ipv4Address = new Column("IPv4Address", "nvarchar(64)", devicesTable);
                ipv4Address.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                devicesTable.Columns.Add(deviceId);
                devicesTable.Columns.Add(ipv4Address);
                Database.Tables.Add(devicesTable);
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Users"))
            {
                Table userTables = new Table("Users");

                Column userId = new Column("UserId", "int", userTables);
                userId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                userId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column userName = new Column("UserName", "nvarchar(64)", userTables);
                userName.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column email = new Column("Email", "nvarchar(128)", userTables);
                email.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column password = new Column("Password", "nvarchar(128)", userTables);
                password.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                userTables.Columns.Add(userId);
                userTables.Columns.Add(userName);
                userTables.Columns.Add(email);
                userTables.Columns.Add(password);
                Database.Tables.Add(userTables);
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Permissions"))
            {
                Table permissionTables = new Table("Permissions");

                Column userId = new Column("UserId", "int", permissionTables);
                userId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Users").First().Columns.Where(column => column.Name == "UserId").First()));

                userId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "first"));
                userId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column deviceId = new Column("DeviceId", "int", permissionTables);
                userId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Devices").First().Columns.Where(column => column.Name == "DeviceId").First()));
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "second"));
                deviceId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                permissionTables.Columns.Add(userId);
                permissionTables.Columns.Add(deviceId);
                Database.Tables.Add(permissionTables);
            }
            _database.SaveDatabaseInfrastructure();
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
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
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