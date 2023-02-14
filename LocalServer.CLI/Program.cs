using System.Text.Json.Nodes;
using System.Text.Json;
using System.Diagnostics;
using System.Net;
using LocalServer.BLL.Server.BLL;
using LocalServer.BLL.DataManipulation.BLL;
using System.Reflection.Emit;
using LocalSerevr.DAL;
using System.Data;

namespace LocalServer.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerLogic server = new ServerLogic(5400, 200 * 60 * 1000);
            server.ServerSetUp();

            string userName = Console.ReadLine();
            string password = Console.ReadLine();

            GlobalServerComunicationLogic.SetUpConnection(userName, password);
            Task.Run(() => GlobalServerComunicationLogic.AwaitServerCall());

            Console.ReadKey();
        }
    }
}