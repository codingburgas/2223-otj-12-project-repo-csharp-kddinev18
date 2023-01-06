using DataAccessLayer;
using LocalServerLogic;
using System.Text.Json.Nodes;
using System.Text.Json;
using LocalServerBusinessLogic;
using System.Diagnostics;

namespace LocalServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseInitialiser databaseIntialiser = new DatabaseInitialiser(200 * 60 * 1000);

            ServerLogic server = new ServerLogic(5400);
            server.ServerSetUp(200 * 60 * 1000);

            while (Console.ReadLine() != "q")
            {
                switch (Console.ReadLine())
                {
                    case "a":
                        server.AprooveClient(Console.ReadLine());
                        break;
                    default:
                        break;
                }
            }
            Console.ReadKey();
            server.ServerShutDown();
        }
    }
}