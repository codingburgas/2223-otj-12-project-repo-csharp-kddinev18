using DataAccessLayer;
using LocalServerLogic;
using System.Text.Json.Nodes;
using System.Text.Json;
using LocalServerBusinessLogic;
using System.Diagnostics;
using System.Net;

namespace LocalServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerLogic server = new ServerLogic(5400);
            server.ServerSetUp(200 * 60 * 1000);

            Console.ReadKey();
            server.ServerShutDown();
        }
    }
}