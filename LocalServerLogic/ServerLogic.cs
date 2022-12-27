using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Security;
using System.Diagnostics;

namespace LocalServerLogic
{
    public class ServerLogic
    {
        private static TcpListener _tcpListener;
        private static List<TcpClient> _clients = new List<TcpClient>();
        private static Dictionary<TcpClient, bool?> _aproovedClients = new Dictionary<TcpClient, bool?>();
        public static List<string> BanList { get; set; } = new List<string>();
        // Buffer
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
            _tcpListener.Stop();
            _tcpListener = null;
        }

        public static void AcceptClients(IAsyncResult asyncResult)
        {
            // Newly connection client
            TcpClient client = null;
            try
            {
                // Connect the client
                client = _tcpListener.EndAcceptTcpClient(asyncResult);
                if(BanList.Contains(((IPEndPoint)client.Client.RemoteEndPoint).ToString()))
                {
                    DisconnectClient(client);
                }
                _aproovedClients.Add(client, null);
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
        }

        public static void ReciveClientInput(IAsyncResult asyncResult)
        {
            TcpClient client = asyncResult.AsyncState as TcpClient;
            if (_aproovedClients[client] == null)
            {
                throw new Exception("You are not authorised to send data");
            }
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
            client.Client.Shutdown(SocketShutdown.Both);
            client.Client.Close();
            _clients.Remove(client);
        }

        public void AprooveClient(TcpClient client)
        {
            _aproovedClients[client] = true;
        }
        public void BanClient(TcpClient client)
        {
            BanList.Add(((IPEndPoint)client.Client.RemoteEndPoint).ToString());
            _aproovedClients.Remove(client);
            DisconnectClient(client);
        }
    }
}