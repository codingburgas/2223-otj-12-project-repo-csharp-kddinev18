using DataAccessLayer;
using LocalServerLogic;
using System.Text.Json.Nodes;
using System.Text.Json;
using LocalServerBusinessLogic;

namespace LocalServer
{
    internal class Program
    {

        static void Main(string[] args)
        {
            DatabaseIntialiser databaseIntialiser = new DatabaseIntialiser(200 * 60 * 1000);
            ClientHandlingLogic clientHandlingLogic = new ClientHandlingLogic(databaseIntialiser);
            ServerLogic server = new ServerLogic(5400, clientHandlingLogic);
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