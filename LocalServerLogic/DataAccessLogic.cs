using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LocalServerLogic
{
    public class DataAccessLogic
    {
        private Dictionary<string, TcpClient> _clientsTables = new Dictionary<string, TcpClient>();
        private SqlConnection _sqlConnection;
        private string _connectionSting = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True";
        public void InitialiseDatabase()
        {
            _sqlConnection = new SqlConnection(_connectionSting);
            _sqlConnection.Open();
            string query = "SELECT name FROM sys.tables";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _clientsTables.Add(reader[0].ToString(), null);
                    }
                }
            }
            if(!_clientsTables.ContainsKey("Devices"))
            {
                query = "CREATE TABLE Devices (Id int IDENTITY(1,1) PRIMARY KEY NOT NULL, InternetProtocolAddress nvarchar(32) NOT NULL UNIQUE, IsBanned bit NULL)";
                using (SqlCommand command = new SqlCommand(query, _sqlConnection))
                {
                    //return command.ExecuteNonQuery() == 1;
                }
            }
        }
        public bool? IsClientBanned(TcpClient client)
        {
            string internetProtocolAddress = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
            string query = "SELECT IsBanned FROM Devices WHERE InternetProtocolAddress = @IPAddress";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@IPAddress", SqlDbType.NVarChar).Value = internetProtocolAddress;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        return bool.Parse(reader[0].ToString());
                    }
                    else
                    {
                        if (AddClientDevice(internetProtocolAddress))
                            return null;
                        else
                            throw new Exception("Can not add the client device to the database");
                    }
                }
            }
        }
        private bool AddClientDevice(string internetProtocolAddress)
        {
            string query = "INSERT INTO Devices VALUES(@IPAddress, null)";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@IPAddress", SqlDbType.NVarChar).Value = internetProtocolAddress;
                return command.ExecuteNonQuery() == 1;
            }
        }

        public bool AprooveClients(TcpClient client)
        {
            string internetProtocolAddress = ((IPEndPoint)client.Client.RemoteEndPoint).ToString();
            string query = "INSERT INTO Devices VALUES(@IPAddress, null)";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@IPAddress", SqlDbType.NVarChar).Value = internetProtocolAddress;
                return command.ExecuteNonQuery() == 1;
            }
        }

        private void CreateTable()
        {

        }
        private void InsertData()
        {

        }
    }
}
