namespace LocalServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServerLogic server = new ServerLogic(5400);
            server.ServerSetUp();
            Console.ReadKey();
            server.ServerShutDown();
        }
    }
}