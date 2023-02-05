using GlobalServer.BLL.Server.BLL;

namespace GlobalServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerLogic server = new ServerLogic(5401);
            server.ServerSetUp(1000 * 200 * 60);
            Console.ReadKey();

            ServerLogic.LocalServerCommunication(4, "HI");
            Console.ReadKey();
        }
    }
}