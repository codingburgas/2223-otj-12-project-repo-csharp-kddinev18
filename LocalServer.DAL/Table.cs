using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Xml.Serialization;

namespace LocalSerevr.DAL
{
    public class Table
    {
        public Database Database { get; set; }
        public string Name { get; set; }
        public HashSet<Column> Columns { get; set; }

        private string _insertQueryContainer = String.Empty;
        private bool _isDataInserted = false;

        public Table(string name, Database database)
        {
            Name = name;
            Columns = new HashSet<Column>();
            Database = database;
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
            _isDataInserted = false;
        }
        public void Create()
        {
            string columns = String.Join(' ', Columns.Select(column => column.ToString()));
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
        public DataTable Select(string columnName = "", string expression = "", string value = "", int pagingSize = 0, int skipAmount = 0)
        {
            string query = String.Empty;
            string paging = pagingSize != 0 ? $"ORDER BY {String.Join(", ", FindPrimaryKeys().Select(column => $"[{column.Name}]"))} OFFSET ({skipAmount}) ROWS FETCH NEXT ({pagingSize}) ROWS ONLY" : "";
            string columns = String.Join(", ", Columns.Select(column => $"[{column.Name}]"));

            if (columnName == String.Empty && expression == String.Empty && value == String.Empty)
            {
                query = $"SELECT {columns} FROM [{Name}] {paging};";
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

            query = $"SELECT {columns} FROM [{Name}] WHERE [{columnName}] {expression} @Value {paging}";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                int integerContainer = 0;
                float floatContainer = 0;
                if (int.TryParse(value, out integerContainer))
                {
                    command.Parameters.Add("@Value", SqlDbType.Int).Value = integerContainer;
                }
                else if (float.TryParse(value, out floatContainer))
                {
                    command.Parameters.Add("@Value", SqlDbType.Decimal).Value = floatContainer;
                }
                else if (value == "NULL")
                {
                    command.Parameters.AddWithValue("@Value", null);
                }
                else
                {
                    command.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value;
                }
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }
        public int GetRowsCount()
        {
            string query = $"SELECT COUNT({FindPrimaryKeys().Select(column => column.Name).First()}) FROM [{Name}]";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return int.Parse(reader[0].ToString());
                    else
                        return 0;
                }
            }
        }
        public void Delete(string columnName = "", string expression = "", string value = "", bool isTrusted = false, string whereCaluse = "")
        {
            string query = String.Empty;
            if(whereCaluse != String.Empty && isTrusted)
            {
                query = $"DELETE FROM [{Name}] WHERE {whereCaluse};";
                using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
                {
                    command.ExecuteNonQuery();
                }
                return;
            }
            if (columnName == String.Empty && expression == String.Empty && value == String.Empty)
            {
                query = $"DELETE FROM [{Name}];";
                using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
                {
                    command.ExecuteNonQuery();
                }
                return;
            }
            if (!Columns.Select(column => column.Name).Contains(columnName))
                throw new Exception($"The column with name {columnName} does not exist in table {Name}");
            if (expression != "=" && expression != "!=" && expression != "<" && expression != ">" && expression != ">=" && expression != "<=")
                throw new Exception($"The operation {expression} is not supported");

            query = $"DELETE FROM [{Name}] WHERE [{columnName}] {expression} @Value;";
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                int integerContainer = 0;
                float floatContainer = 0;
                if (isTrusted)
                {
                    query = $"DELETE FROM [{Name}] WHERE [{columnName}] {expression} {value};";
                }
                else
                {
                    if (int.TryParse(value, out integerContainer))
                    {
                        command.Parameters.Add("@Value", SqlDbType.Int).Value = integerContainer;
                    }
                    else if (float.TryParse(value, out floatContainer))
                    {
                        command.Parameters.Add("@Value", SqlDbType.Decimal).Value = floatContainer;
                    }
                    else if (value == "NULL")
                    {
                        command.Parameters.AddWithValue("@Value", null);
                    }
                    else
                    {
                        command.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value;
                    }
                }
                command.ExecuteNonQuery();
            }
        }
        public void Insert(params string[] data)
        {
            if (_insertQueryContainer == String.Empty)
            {
                string columns = String.Empty;
                foreach (Column column in Columns)
                {
                    if (column.Constraints.Any(constraint => constraint.Item1 == "PRIMARY KEY" || constraint.Item1 == "DEFAULT") &&
                        !column.Constraints.Any(constraint => constraint.Item1 == "FOREIGN KEY"))
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
        public void Update(string updateColumnName, string updateValue, string conditionColumnName, string expression, string coditionValue, bool isTrusted = false, string whereClause = "")
        {
            string query = String.Empty;
            if (!isTrusted)
            {
                if (!Columns.Select(column => column.Name).Contains(updateColumnName))
                    throw new Exception($"The column with name {updateColumnName} does not exist in table {Name}");
                if (!Columns.Select(column => column.Name).Contains(conditionColumnName))
                    throw new Exception($"The column with name {conditionColumnName} does not exist in table {Name}");
                if (expression != "=" && expression != "!=" && expression != "<" && expression != ">" && expression != ">=" && expression != "<=")
                    throw new Exception($"The operation {expression} is not supported");
            }

            if(isTrusted)
            {
                query = $"UPDATE {Name} SET {updateColumnName} = @UpdateValue WHERE {whereClause};";
            }
            else
            {
                query = $"UPDATE {Name} SET {updateColumnName} = @UpdateValue WHERE {conditionColumnName} {expression} @ConditionValue;";
            }
            using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
            {
                int integerContainer = 0;
                float floatContainer = 0;
                if (int.TryParse(updateValue, out integerContainer))
                {
                    command.Parameters.Add("@UpdateValue", SqlDbType.Int).Value = integerContainer;
                }
                else if (float.TryParse(updateValue, out floatContainer))
                {
                    command.Parameters.Add("@UpdateValue", SqlDbType.Decimal).Value = floatContainer;
                }
                else if (updateValue == "NULL")
                {
                    command.Parameters.AddWithValue("@UpdateValue", null);
                }
                else
                {
                    command.Parameters.Add("@UpdateValue", SqlDbType.NVarChar).Value = updateValue;
                }
                if (!isTrusted)
                {

                    if (int.TryParse(coditionValue, out integerContainer))
                    {
                        command.Parameters.Add("@ConditionValue", SqlDbType.Int).Value = integerContainer;
                    }
                    else if (float.TryParse(coditionValue, out floatContainer))
                    {
                        command.Parameters.Add("@ConditionValue", SqlDbType.Decimal).Value = floatContainer;
                    }
                    else if (coditionValue == "NULL")
                    {
                        command.Parameters.AddWithValue("@ConditionValue", null);
                    }
                    else
                    {
                        command.Parameters.Add("@ConditionValue", SqlDbType.NVarChar).Value = coditionValue;
                    }
                }

                command.ExecuteNonQuery();
            }
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
                if (column.Constraints.Select(constraint => constraint.Item1).Contains("PRIMARY KEY"))
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