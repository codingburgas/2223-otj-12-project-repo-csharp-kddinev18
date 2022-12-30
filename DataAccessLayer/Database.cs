using System.Data;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class Database
    {
        public static HashSet<Table> Tables { get; set; }

        private static SqlConnection _sqlConnection;
        private static string _connectionString;
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
        public static SqlConnection GetConnection()
        {
            return _sqlConnection;
        }
        public void CloseConnection()
        {
            _connectionString = String.Empty;
            _sqlConnection.Close();
        }

        public void LoadDatabaseInfrastructure()
        {
            Tables = new HashSet<Table>();
            string query = "SELECT name " +
                           "FROM sys.tables";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tables.Add(new Table(reader[0].ToString()));
                    }
                }
            }

            foreach (Table table in Tables)
            {
                LoadTableColumns(table);
            }

            foreach (Table table in Tables)
            {
                LoadPrimaryKeys(table);
            }

            foreach (Table table in Tables)
            {
                LoadForeignKeys(table);
            }
        }

        private void LoadTableColumns(Table table)
        {
            string query = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT, CHARACTER_MAXIMUM_LENGTH " +
                           "FROM INFORMATION_SCHEMA.COLUMNS " +
                           "WHERE TABLE_NAME = @TableName;";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SqlDbType dbType;
                        if (Enum.TryParse<SqlDbType>(reader[1].ToString(), true, out dbType))
                        {
                            Column column = new Column(reader[0].ToString(), reader[4].ToString() == "" || dbType.ToString() == "Image" || dbType.ToString() == "Text"
                                ? dbType.ToString() : dbType.ToString() + $"({reader[4].ToString()})", table);
                            if (reader[2].ToString() == "NO")
                                column.AddConstraint(new Tuple<string, object>("NOT NULL", null));
                            if (reader[3].ToString() != "")
                                column.AddConstraint(new Tuple<string, object>("DEFAULT", reader[3].ToString()));

                            query = "SELECT CC.COLUMN_NAME " +
                                    "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC " +
                                    "JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE CC " +
                                    "ON TC.CONSTRAINT_NAME = CC.CONSTRAINT_NAME " +
                                    "WHERE TC.CONSTRAINT_TYPE = 'Unique' AND TC.TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";
                            using (SqlCommand innerCommand = new SqlCommand(query, _sqlConnection))
                            {
                                innerCommand.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                                innerCommand.Parameters.Add("@ColumnName", SqlDbType.NVarChar).Value = column.Name;
                                using (SqlDataReader innerReader = innerCommand.ExecuteReader())
                                {
                                    if (innerReader.Read())
                                    {
                                        column.AddConstraint(new Tuple<string, object>("UNIQUE", null));
                                    }

                                }
                            }

                            query = "SELECT ac.name, cc.Definition " +
                                    "FROM sys.check_constraints cc " +
                                    "LEFT JOIN sys.objects o " +
                                    "ON cc.parent_object_id = o.object_id " +
                                    "LEFT JOIN sys.all_columns ac " +
                                    "ON cc.parent_column_id = ac.column_id AND cc.parent_object_id = ac.object_id " +
                                    "WHERE o.name = @TableName AND ac.name = @ColumnName";
                            using (SqlCommand innerCommand = new SqlCommand(query, _sqlConnection))
                            {
                                innerCommand.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                                innerCommand.Parameters.Add("@ColumnName", SqlDbType.NVarChar).Value = column.Name;
                                using (SqlDataReader innerReader = innerCommand.ExecuteReader())
                                {
                                    if (innerReader.Read())
                                    {
                                        column.AddConstraint(new Tuple<string, object>("CHECK", innerReader[1].ToString()));
                                    }
                                }
                            }

                            table.Columns.Add(column);
                        }
                        else
                        {
                            throw new ArgumentException($"{reader[1].ToString()} is not a valid SQL database type");
                        }
                    }
                }
            }
        }

        private void LoadPrimaryKeys(Table table)
        {
            string query = "SELECT COLUMN_NAME " +
                           "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                           "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 " +
                           "AND TABLE_NAME = @TableName";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        table.Columns.Where(column => column.Name == reader[0].ToString()).First().AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                    }
                    else
                    {
                        throw new ArgumentException($"The table {table.Name} does not have a primry key");
                    }
                }
            }
        }

        private void LoadForeignKeys(Table table)
        {
            string query = "SELECT OBJECT_NAME(f.parent_object_id), COL_NAME(fc.parent_object_id,fc.parent_column_id) " +
                                    "FROM sys.foreign_keys AS f " +
                                    "JOIN sys.foreign_key_columns AS fc " +
                                    "ON f.OBJECT_ID = fc.constraint_object_id " +
                                    "JOIN sys.tables t " +
                                    "ON t.OBJECT_ID = fc.referenced_object_id " +
                                    "WHERE OBJECT_NAME (f.referenced_object_id) = @TableName";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Table foreignKeyTable = Tables.Where(table => table.Name == reader[0].ToString()).First();
                        Column foreignKeyColumn = foreignKeyTable.Columns.Where(column => column.Name == reader[1].ToString()).First();
                        foreignKeyColumn.AddConstraint(new Tuple<string, object>("FOREIGN KEY", FindPrimaryKeys(table)));
                    }
                }
            }
        }

        private Column FindPrimaryKeys(Table table)
        {
            string query = "SELECT COLUMN_NAME " +
                           "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE " +
                           "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 " +
                           "AND TABLE_NAME = @TableName";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return table.Columns.Where(column => column.Name == reader[0].ToString()).First();
                    }
                    else
                    {
                        throw new ArgumentException($"The table {table.Name} does not have a primry key");
                    }
                }
            }
        }
    }
}