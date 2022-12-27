using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
            if(_clientsTables.ContainsKey("BanList"))
            {
                query = "SELECT [IP] FROM BanList";
                using (SqlCommand command = new SqlCommand(query, _sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ServerLogic.BanList.Add(reader[0].ToString())
                        }
                    }
                }
            }
            else
            {

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
