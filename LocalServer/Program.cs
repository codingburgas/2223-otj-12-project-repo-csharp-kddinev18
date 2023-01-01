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
            ServerLogic server = new ServerLogic(5400);
            server.ServerSetUp(20 * 60 * 1000);
            Console.ReadKey();
            server.ServerShutDown();
        }
    }
}