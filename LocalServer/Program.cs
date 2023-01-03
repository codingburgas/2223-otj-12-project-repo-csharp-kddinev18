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
            DatabaseIntialiser databaseIntialiser = new DatabaseIntialiser(200 * 60 * 1000);

            ClientHandlingLogic clientHandlingLogic = new ClientHandlingLogic(databaseIntialiser);
            ServerLogic server = new ServerLogic(5400, clientHandlingLogic);
            server.ServerSetUp(200 * 60 * 1000);
            UserAuthenticationLogic userAuthenticationLogic = new UserAuthenticationLogic(databaseIntialiser);
            //int id = userAuthenticationLogic.Register("Milko12345", "Milko@abv.bg", "Password1!1!");

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