using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace LocalSerevr.DAL
{
    public class Database
    {
        public HashSet<Table> Tables { get; set; }

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
        public Database(string connectionString)
        {
            Tables = new HashSet<Table>();
            _connectionString = connectionString;
            if (_sqlConnection == null)
            {
                _sqlConnection = new SqlConnection(_connectionString);
                _sqlConnection.Open();
            }
        }
        public Database()
        {
            Tables = new HashSet<Table>();
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Tables = new HashSet<Table>();
            string query = "SELECT name " +
                           "FROM sys.tables";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tables.Add(new Table(reader[0].ToString(), this));
                    }
                }
            }
            foreach (Table table in Tables)
            {
                LoadTableColumns(table);
            }

            foreach (Table table in Tables)
            {
                LoadKeys(table);
            }

            foreach (Table table in Tables)
            {
                LoadForeignKeys(table);
            }
            stopwatch.Stop();
            Console.Write(stopwatch.Elapsed);
        }

        private void LoadTableColumns(Table table)
        {
            Stopwatch stopwatch = new Stopwatch();
            
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

        private void LoadKeys(Table table)
        {
            List<Column> primaryKerys = table.FindPrimaryKeysThroughQuery();
            foreach (Column col in primaryKerys)
            {
                if(primaryKerys.Count > 1)
                {
                    table.Columns.Where(column => column.Name == col.Name).First().Constraints.Add(new Tuple<string, object>("PRIMARY KEY", "multiple"));
                }
                else
                {
                    table.Columns.Where(column => column.Name == col.Name).First().Constraints.Add(new Tuple<string, object>("PRIMARY KEY", null));
                }
            }
            string query = "SELECT COL.NAME FROM " +
                    "SYS.OBJECTS OBJ " +
                    "JOIN SYS.COLUMNS COL ON COL.[OBJECT_ID] = OBJ.[OBJECT_ID] " +
                    "JOIN SYS.INDEX_COLUMNS IDX_COLS ON IDX_COLS.[COLUMN_ID] = COL.[COLUMN_ID] AND IDX_COLS.[OBJECT_ID] = COL.[OBJECT_ID] " +
                    "JOIN SYS.INDEXES IDX ON IDX_COLS.[INDEX_ID] = IDX.[INDEX_ID] AND IDX.[OBJECT_ID] = COL.[OBJECT_ID] " +
                    "WHERE OBJ.NAME = @TableName " +
                    "AND IDX.IS_UNIQUE = 1 AND IDX.IS_PRIMARY_KEY = 0";
            using (SqlCommand command = new SqlCommand(query, _sqlConnection))
            {
                command.Parameters.Add("@TableName", SqlDbType.NVarChar).Value = table.Name;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        table.Columns.Where(column => column.Name == reader[0].ToString()).First().AddConstraint(new Tuple<string, object>("UNIQUE", null));
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
                        foreignKeyColumn.AddConstraint(new Tuple<string, object>("FOREIGN KEY", table.FindPrimaryKeysThroughQuery().First()));
                    }
                }
            }
        }

        public void SaveDatabaseInfrastructure()
        {
            HashSet<Table> newInfrastructure = Tables;
            LoadDatabaseInfrastructure();
            HashSet<Table> oldInfrastructure = Tables;

            SaveTableChanges(newInfrastructure, oldInfrastructure);
            SaveColumnChanges(newInfrastructure, oldInfrastructure);
        }

        private void SaveTableChanges(HashSet<Table> newInfrastructure, HashSet<Table> oldInfrastructure)
        {
            string query = String.Empty;
            IEnumerable<string> added = newInfrastructure.Select(table => table.Name).Except(oldInfrastructure.Select(table => table.Name));
            foreach (string tableName in added)
            {
                newInfrastructure.Where(table => table.Name == tableName).First().Create();
            }
            IEnumerable<string> removed = oldInfrastructure.Select(table => table.Name).Except(newInfrastructure.Select(table => table.Name));
            foreach (string tableName in removed)
            {
                oldInfrastructure.Where(table => table.Name == tableName).First().Drop();
            }
        }

        private void SaveColumnChanges(HashSet<Table> newInfrastructure, HashSet<Table> oldInfrastructure)
        {
            string query = String.Empty;
            IEnumerable<string> tablesNames = newInfrastructure.Select(table => table.Name).Except(
                newInfrastructure.Select(table => table.Name).Except(oldInfrastructure.Select(table => table.Name)).Union(
                    oldInfrastructure.Select(table => table.Name).Except(newInfrastructure.Select(table => table.Name))));

            foreach (string tableName in tablesNames)
            {
                Table newTable = newInfrastructure.Where(table => table.Name == tableName).First();
                Table oldTable = oldInfrastructure.Where(table => table.Name == tableName).First();
                IEnumerable<string> added = newTable.Columns.Select(column => column.Name).Except(oldTable.Columns.Select(column => column.Name));
                foreach (string columnName in added)
                {
                    query = $"ALTER TABLE [{newTable.Name}] ADD [COLUMN] {newTable.Columns.Where(column => column.Name == columnName).First()}";
                }
                IEnumerable<string> removed = newTable.Columns.Select(column => column.Name).Except(oldTable.Columns.Select(column => column.Name));
                foreach (string columnName in removed)
                {
                    query = $"ALTER TABLE [{oldTable.Name}] DROP COLUMN [{columnName}];";
                }
            }
        }

        public void SaveDatabaseData()
        {
            foreach (Table table in Tables)
            {
                if (table.IsDataInserted())
                {
                    string query = table.GetInsertQuery();
                    query = query.Substring(0, query.Length - 1)+';';
                    using (SqlCommand command = new SqlCommand(query, Database.GetConnection()))
                    {
                        command.ExecuteNonQuery();
                    }
                    table.DiscardInsertQuery();
                }
            }
        }

        public void DiscardData()
        {
            foreach (Table table in Tables)
            {
                table.DiscardInsertQuery();
            }
        }
    }
}