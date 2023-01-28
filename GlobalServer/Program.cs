using System.Net.Sockets;
using System.Net;
using System.Text;

namespace GlobalServer
{
    public class ServerLogic
    {
        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static Dictionary<string, bool> _aproovedClients = new Dictionary<string, bool>();

        private static byte[] _data = new byte[16777216];

        private int _port;
        private static int _success = 0;
        private static int _error = 1;

        public ServerLogic(int port)
        {
            _port = port;
        }
        public void ServerSetUp()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, _port);
                // Starts the server
                _tcpListener.Start();
                // Starts accepting clients
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ServerShutDown();
            }
        }

        public void ServerShutDown()
        {
            // Stops the server
            if (_tcpListener != null)
                _tcpListener.Stop();
            foreach (TcpClient client in _clients)
            {
                DisconnectClient(client);
            }
            _tcpListener = null;
        }

        public static void AcceptClients(IAsyncResult asyncResult)
        {
            // Newly connection client
            TcpClient client = null;
            try
            {
                // Connect the client
                if (_tcpListener is not null)
                {
                    client = _tcpListener.EndAcceptTcpClient(asyncResult);
                    _aproovedClients.Add(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), false);
                    Console.WriteLine("Client connected with IP {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Add the client newly connect client into the _clients list
            _clients.Add(client);
            // Begin recieving bytes from the client
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
            _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            if (_tcpListener is not null)
            {
                DisconnectClient(client);
                _tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptClients), null);
            }
        }

        public static void ReciveClientInput(IAsyncResult asyncResult)
        {
            TcpClient client = asyncResult.AsyncState as TcpClient;
            int reciever;
            try
            {
                // How many bytes has the user sent
                reciever = client.Client.EndReceive(asyncResult);
                // If the bytes are - disconnect the client
                if (reciever == 0)
                {
                    DisconnectClient(client);
                    return;
                }
                // Get the data
                string data = Encoding.ASCII.GetString(_data).Replace("\0", String.Empty);

                if (_aproovedClients[((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()] == false)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                string response = $"{_error}|{ex.Message}";
                // send data to the client
                client.Client.Send(Encoding.ASCII.GetBytes(response));
            }
            finally
            {
                FlushBuffer();
            }
            client.Client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(ReciveClientInput), client);
        }

        // Clear the buffer
        public static void FlushBuffer()
        {
            Array.Clear(_data, 0, _data.Length);
        }

        public static void DisconnectClient(TcpClient client)
        {
            Console.WriteLine("Client disconnected");
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
            _clients.Remove(client);
            _aproovedClients.Remove(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
            client = null;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}