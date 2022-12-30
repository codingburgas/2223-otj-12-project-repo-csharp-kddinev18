using System.Data.Common;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class Table
    {
        public string Name { get; set; }
        public HashSet<Column> Columns { get; set; }

        public Table(string name)
        {
            Name = name;
            Columns = new HashSet<Column>();
        }
        public void Create()
        {
            string columns = String.Empty;
            foreach (Column column in Columns)
            {
                columns += column.ToString();
            }
            string query = $"CREATE TABLE [{Name}] \n(\n{columns});";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                command.ExecuteNonQuery();
            }
            Database.Tables.Add(this);
        }
    }
}