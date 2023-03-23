using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using System.Data;
using System.Net.Http;
using System.Xml.Linq;
using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.DAL;

namespace LocalServer.BLL.Server.BLL
{
    public static class ClientHandlingLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }
        public static void HandleClientInput(string data, string ipAddress, List<TcpClient> clients)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
            if (jObject["Operation"].ToString() == "Authenticate")
            {
                CreateDeviceTable(ipAddress, jObject);
            }
            else if (jObject["Operation"].ToString() == "Insert")
            {
                InsertDeviceData(ipAddress, jObject);
            }
            else
            {
                throw new Exception("Wrong operation type");
            }
        }

        private static void InsertDeviceData(string ipAddress, JsonObject jObject)
        {
            Table table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == jObject["Name"].ToString()).First();
            List<string> insertData = new List<string>();

            List<string> args = JsonSerializer.Deserialize<List<string>>(jObject["Columns"].ToString());
            args.Add(DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Select("IPv4Address", "=", ipAddress).Rows[0]["DeviceId"].ToString());

            table.Insert(args.ToArray());
            DatabaseInitialiser.Database.SaveDatabaseData();
        }

        private static void CreateDeviceTable(string ipAddress, JsonObject jObject)
        {
            if (!DatabaseInitialiser.Database.Tables.Select(table => table.Name).Contains(jObject["Name"].ToString()))
            {
                return;
            }

            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Update("Name", jObject["Name"].ToString(), "IPv4Address", "=", ipAddress);

            Table newTable = new Table(jObject["Name"].ToString(), DatabaseInitialiser.Database);
            foreach (JsonObject column in JsonSerializer.Deserialize<List<JsonObject>>(jObject["Columns"].ToString()))
            {
                Column newColumn = new Column(column["Name"].ToString(), column["Type"].ToString(), newTable);
                foreach (JsonObject constraint in JsonSerializer.Deserialize<List<JsonObject>>(column["Constraints"].ToString()))
                {
                    if (constraint["Constraint"].ToString() == "FOREIGN KEY")
                    {
                        string tableName = constraint["AdditionalInformation"].ToString().Split(", ")[0];
                        string columnsName = constraint["AdditionalInformation"].ToString().Split(", ")[1];
                        Column foreignKeyColumn = DatabaseInitialiser.Database.Tables.Where(table => table.Name == tableName).First()
                            .Columns.Where(column => column.Name == columnsName).First();
                        newColumn.AddConstraint(new Tuple<string, object>(constraint["Constraint"].ToString(), foreignKeyColumn));
                    }
                    else
                    {
                        newColumn.AddConstraint(new Tuple<string, object>(
                            constraint["Constraint"].ToString(), 
                            constraint["AdditionalInformation"].ToString())
                        );
                    }
                }
                newTable.Columns.Add(newColumn);
            }
            Column systemTimeColumn = new Column("Created", "datetime2(7)", newTable);
            systemTimeColumn.AddConstraint(new Tuple<string, object>("DEFAULT", "GETDATE()"));

            Column systemColumn = new Column("DeviceId", "int", newTable);
            systemColumn.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().FindPrimaryKeys().First()));

            newTable.Columns.Add(systemTimeColumn);
            newTable.Columns.Add(systemColumn);

            DatabaseInitialiser.Database.Tables.Add(newTable);
            DatabaseInitialiser.Database.SaveDatabaseInfrastructure();
        }

        public static bool AddClients(TcpClient client)
        {
            string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string devicesJson = Table.ConvertDataTabletoString(DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Devices").First().Select("IPv4Address", "=", clientIpAddress));
            List<JsonObject> devices = JsonSerializer.Deserialize<List<JsonObject>>(devicesJson);
            if (devices.Count == 0)
            {
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                    .Insert(clientIpAddress, clientIpAddress, "false");
                DatabaseInitialiser.Database.SaveDatabaseData();

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
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Update("IsAprooved", "true", "IPv4Address", "=", ipAddress);

            string deviceId = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Select("IPv4Address", "=", ipAddress).Rows[0]["DeviceId"].ToString();

            foreach (DataRow item in DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First().Select().Rows)
            {
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First()
                    .Insert(item["RoleId"].ToString(), deviceId, "false", "false", "false", "false");
            }
            DatabaseInitialiser.Database.SaveDatabaseData();
        }
    }
}
