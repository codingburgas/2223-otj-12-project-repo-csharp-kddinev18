using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class Database
    {
        public static HashSet<Table> Tables { get; set; }

        private SqlConnection _sqlConnection;
        private string _connectionString;
        public void InitializeDatabase(string connectionString)
        {
            Tables = new HashSet<Table>();
            _connectionString = connectionString;
            if (_sqlConnection == null)
            {
                _sqlConnection = new SqlConnection(_connectionString);
                _sqlConnection.Open();
            }
        }
    }
}