using LocalServer.BLL.Server.BLL;

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

            BridgeAPIHandlingLogic.SetUpConnection(userName, password);
            Task.Run(() => BridgeAPIHandlingLogic.AwaitServerCall());

            Console.ReadKey();
        }
    }
}