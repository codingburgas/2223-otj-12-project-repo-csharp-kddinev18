using DataAccessLayer;
using LocalServerLogic;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace LocalServer
{
    internal class Program
    {
        private static string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ORMTest;Integrated Security=True;MultipleActiveResultSets=true";

        static void Main(string[] args)
        {
            /*ServerLogic server = new ServerLogic(5400);
            server.ServerSetUp();
            Console.ReadKey();
            server.ServerShutDown();*/
            Database _database = new Database(_connectionString);
            _database.LoadDatabaseInfrastructure();

            string clientName = "TestName";
            string clientIpAddress = "TestIPAddress";


            string jsonString = Table.ConvertDataTabletoString(Database.Tables.Where(table => table.Name == "Devices").First().Select("IPv4Address", "=", clientIpAddress));
            List<JsonObject> jObject = JsonSerializer.Deserialize<List<JsonObject>>(jsonString);
            if (jObject.Count == 0)
            {
                Database.Tables.Where(table => table.Name == "Devices").First().Insert(clientIpAddress, clientName, "false");
                _database.SaveDatabaseData();
            }
            else
            {
                if (bool.Parse(jObject.First()["IsAprooved"].ToString()) == false)
                {
                    Console.WriteLine("NOOOO");
                }
            }
            _database.CloseConnection();
        }
    }
}