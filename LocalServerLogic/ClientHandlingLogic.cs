using DataAccessLayer;
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

namespace LocalServerBusinessLogic
{
    public static class ClientHandlingLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }
        public static void HandleClientInput(string data, List<TcpClient> clients)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(data);
            if (jObject["Operation"].ToString() == "Authenticate")
            {
                if (!DatabaseInitialiser.Database.Tables.Select(table => table.Name).Contains(jObject["Name"].ToString()))
                {
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
                                newColumn.AddConstraint(new Tuple<string, object>(constraint["Constraint"].ToString(), constraint["AdditionalInformation"].ToString()));
                            }
                        }
                        newTable.Columns.Add(newColumn);
                    }
                    Column systemColumn = new Column("Created", "datetime2(7)", newTable);
                    systemColumn.AddConstraint(new Tuple<string, object>("DEFAULT", "GETDATE()"));
                    newTable.Columns.Add(systemColumn);

                    DatabaseInitialiser.Database.Tables.Add(newTable);
                    DatabaseInitialiser.Database.SaveDatabaseInfrastructure();
                }
            }
            else if (jObject["Operation"].ToString() == "Insert")
            {
                Table table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == jObject["Name"].ToString()).First();
                List<string> insertData = new List<string>();

                table.Insert(JsonSerializer.Deserialize<List<string>>(jObject["Columns"].ToString()).ToArray());
                DatabaseInitialiser.Database.SaveDatabaseData();
            }
            else if (jObject["Operation"].ToString() == "Send")
            {
                string ipAddressDestination = jObject["Address"].ToString();
                foreach (TcpClient client in clients)
                {
                    if (((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() == ipAddressDestination)
                    {
                        NetworkStream stream = client.GetStream();
                        string sendingData = jObject["Data"].ToString();
                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        //Send to Client
                        stream.Write(msg, 0, msg.Length);
                    }
                }
            }
        }
        public static bool AddClients(TcpClient client)
        {
            string clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string devicesJson = Table.ConvertDataTabletoString(DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Select("IPv4Address", "=", clientIpAddress));
            List<JsonObject> devices = JsonSerializer.Deserialize<List<JsonObject>>(devicesJson);
            if (devices.Count == 0)
            {
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Insert(clientIpAddress, clientIpAddress, "false");
                DatabaseInitialiser.Database.SaveDatabaseData();

                string deviceId = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                    .Select("IPv4Address", "=", clientIpAddress).Rows[0]["DeviceId"].ToString();

                foreach (DataRow item in DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First().Select().Rows)
                {
                    DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First()
                        .Insert(item["RoleId"].ToString(), deviceId, "false", "false", "false", "false");
                }
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
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Update("IsAprooved", "true", "IPv4Address", "=", ipAddress);
        }
    }
}
