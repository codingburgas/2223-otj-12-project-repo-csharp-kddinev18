using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics;
using System.Net;
using LocalServer.BLL;

namespace LocalServer.CLI
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