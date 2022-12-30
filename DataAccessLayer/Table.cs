using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
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
            string query = $"CREATE TABLE [{Name}] \n(\n{columns}\n PRIMARY KEY ({String.Join(',', FindPrimaryKeys().Select(column => column.Name))})\n);";
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
        public DataTable Select(string columnName = "", string expression = "", string value = "")
        {
            string query = String.Empty;
            string columns = String.Empty;
            foreach (Column column in Columns)
            {
                columns += column.Name + ',';
            }
            columns = columns.Substring(0, columns.Length - 1);
            if (columnName == String.Empty && expression == String.Empty && value == String.Empty)
            {
                query = $"SELECT {columns} FROM {Name};";
                using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
            if (!Columns.Select(column => column.Name).Contains(columnName))
                throw new Exception($"The column with name {columnName} does not exist in table {Name}");
            if (expression != "=" && expression != "!=" && expression != "<" && expression != ">" && expression != ">=" && expression != "<=")
                throw new Exception($"The operation {expression} is not supported");

            query = $"SELECT {columns} FROM {Name} WHERE {columnName} {expression} @Value;";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                int integerContainer = 0;
                float floatContainer = 0;
                if (int.TryParse(value, out integerContainer))
                {
                    command.Parameters.Add("@Value", SqlDbType.Int).Value = integerContainer;
                }
                if (float.TryParse(value, out floatContainer))
                {
                    command.Parameters.Add("@Value", SqlDbType.Decimal).Value = floatContainer;
                }
                if (value == "NULL")
                {
                    command.Parameters.AddWithValue("@Value", null);
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
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
        public List<Column> FindPrimaryKeysThroughQuery()
        {
            string query = "SELECT COLUMN_NAME " +
                           "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                           "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 " +
                           "AND TABLE_NAME = @TableName";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = Name;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<Column> primaryKeys = new List<Column>();
                    while (reader.Read())
                    {
                        primaryKeys.Add(Columns.Where(column => column.Name == reader[0].ToString()).First());
                    }
                    if (primaryKeys.Count == 0)
                    {
                        throw new ArgumentException($"The table {Name} does not have a primry key");
                    }
                    else
                    {
                        return primaryKeys;
                    }
                }
            }
        }
        public List<Column> FindPrimaryKeys()
        {
            List<Column> primaryKeys = new List<Column>();
            foreach (Column column in Columns)
            {
                if(column.Constraints.Select(constraint=>constraint.Item1).Contains("PRIMARY KEY"))
                {
                    primaryKeys.Add(column);
                }
            }
            if (primaryKeys.Count == 0)
            {
                throw new ArgumentException($"The table {Name} does not have a primry key");
            }
            else
            {
                return primaryKeys;
            }
        }
        public static string ConvertDataTabletoString(DataTable table)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in table.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return JsonSerializer.Serialize(rows);
        }
    }
}