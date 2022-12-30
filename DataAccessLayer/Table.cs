using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.Json;

namespace DataAccessLayer
{
    public class Table
    {
        public string Name { get; set; }
        public HashSet<Column> Columns { get; set; }

        private string _insertQueryContainer = String.Empty;
        private bool _isDataInserted = false;

        public Table(string name)
        {
            Name = name;
            Columns = new HashSet<Column>();
        }
        public bool IsDataInserted()
        {
            return _isDataInserted;
        }
        public string GetInsertQuery()
        {
            return _insertQueryContainer;
        }
        public void DiscardInsertQuery()
        {
            _insertQueryContainer = String.Empty;
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
        public void Drop()
        {
            string query = $"DROP TABLE [{Name}];";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                command.ExecuteNonQuery();
            }
            Database.Tables.Remove(this);
        }
        public string Select(string columnName, string expression, string value)
        {
            string columns = String.Empty;
            foreach (Column column in Columns)
            {
                columns += column.Name + ',';
            }
            columns = columns.Substring(0, columns.Length - 1);
            if (!Columns.Select(column => column.Name).Contains(columnName))
                throw new Exception($"The column with name {columnName} does not exist in table {Name}");
            if (expression != "=" && expression != "!=" && expression != "<" && expression != ">" && expression != ">=" && expression != "<=")
                throw new Exception($"The operation {expression} is not supported");

            string query = $"SELECT {columns} FROM {Name} WHERE {columnName} {expression} @Value";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                int integerContainer = 0;
                float floatContainer = 0;
                if (int.TryParse(value, out integerContainer)) 
                {
                    command.Parameters.Add("@Value", SqlDbType.Int).Value = integerContainer;
                }
                if(float.TryParse(value, out floatContainer))
                {
                    command.Parameters.Add("@Value", SqlDbType.Decimal).Value = floatContainer;
                }
                if(value == "NULL")
                {
                    command.Parameters.AddWithValue("@Value", null);
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return JsonSerializer.Serialize(table);
                }
            }
        }
        public void Insert(params string[] data)
        {
            if (_insertQueryContainer == String.Empty)
            {
                string columns = String.Empty;
                foreach (Column column in Columns)
                {
                    if (column.Constraints.Any(constraint => constraint.Item1 == "PRIMARY KEY"))
                        continue;

                    columns += $"[{column.Name}],";
                }
                columns = columns.Substring(0, columns.Length - 1);
                _insertQueryContainer = $"INSERT INTO [{Name}] ({columns}) VALUES ";
            }
            int integerContainer = 0;
            float floatContainer = 0;
            string dataString = String.Empty;
            foreach (string value in data)
            {
                if (int.TryParse(value, out integerContainer) || float.TryParse(value, out floatContainer) || value == "NULL")
                {
                    dataString += $"{value},";
                }
                else
                {
                    dataString += $"\'{value}\',";
                }
            }
            dataString = dataString.Substring(0, dataString.Length - 1);
            _insertQueryContainer += $"({dataString}),";
            _isDataInserted = true;
        }
    }
}