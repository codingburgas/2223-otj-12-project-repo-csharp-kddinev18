using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Timers;
using System.Net.Sockets;
using System.Net;

namespace LocalServerLogic
{
    public class BusinessLogic
    {
        private static Database _database;
        private static long _deleteTimer;

        public static void InitialiseDatabase(string connectionString, long deleteTimer)
        {
            _deleteTimer = deleteTimer;
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = deleteTimer;
            timer.Elapsed += TimerOnElapsed;
            timer.Start();

            _database = new Database(connectionString);
            CreateDefaultDatabaseStructure();
        }
        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (Table table in Database.Tables)
            {
                if (table.Name == "Users" || table.Name == "Devices" || table.Name == "Permissions")
                    continue;

                table.Delete("Created", "<", $"DATEADD(mi,{(int)_deleteTimer / (1000 * 60)},GETDATE())", true);
            }
        }
        private static void CreateDefaultDatabaseStructure()
        {
            _database.LoadDatabaseInfrastructure();
            if (!Database.Tables.Select(table => table.Name).Contains("Devices"))
            {
                Table devicesTable = new Table("Devices");

                Column deviceId = new Column("DeviceId", "int", devicesTable);
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                deviceId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column ipv4Address = new Column("IPv4Address", "nvarchar(64)", devicesTable);
                ipv4Address.AddConstraint(new Tuple<string, object>("NOT NULL", null));
                ipv4Address.AddConstraint(new Tuple<string, object>("UNIQUE", null));

                Column name = new Column("Name", "nvarchar(64)", devicesTable);
                name.AddConstraint(new Tuple<string, object>("NOT NULL", null));
                name.AddConstraint(new Tuple<string, object>("UNIQUE", null));

                Column aprooved = new Column("IsAprooved", "bit", devicesTable);
                aprooved.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                devicesTable.Columns.Add(deviceId);
                devicesTable.Columns.Add(ipv4Address);
                devicesTable.Columns.Add(name);
                devicesTable.Columns.Add(aprooved);
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
                deviceId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Devices").First().Columns.Where(column => column.Name == "DeviceId").First()));
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "second"));
                deviceId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                permissionTables.Columns.Add(userId);
                permissionTables.Columns.Add(deviceId);
                Database.Tables.Add(permissionTables);
            }
            _database.SaveDatabaseInfrastructure();
        }

        public static void HandleClientInput(string data)
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
        public static bool AddClients(TcpClient client)
        {
            string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string devicesJson = Table.ConvertDataTabletoString(Database.Tables.Where(table => table.Name == "Devices").First().Select("IPv4Address", "=", clientIpAddress));
            List<JsonObject> devices = JsonSerializer.Deserialize<List<JsonObject>>(devicesJson);
            if (devices.Count == 0)
            {
                Database.Tables.Where(table => table.Name == "Devices").First().Insert(clientIpAddress, clientIpAddress, "false");
                _database.SaveDatabaseData();
                return false;
            }
            else
            {
                if (bool.Parse(devices.First()["IsAprooved"].ToString()) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static void AprooveClient(string ipAddress)
        {
            Database.Tables.Where(table => table.Name == "Devices").First().Update("IsAprooved", "true", "IPv4Address", "=", ipAddress);
        }
    }
}
