using LocalServer.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.TESTS.ORM
{
    public class TableTests
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurityTest;Integrated Security=True;MultipleActiveResultSets=true";

        [TestCase("Persons")]
        [TestCase("Users")]
        [TestCase("Trains")]
        [TestCase("Roles")]
        public void Should_CreateTableInTheDatabase_When_InvokingCreateMethod(string tableName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table(tableName, database);
            Column column = new Column("Id", "int", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            // Act
            table.Create();

            List<string> tables = new List<string>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                string query = "SELECT name " +
                               "FROM sys.tables";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader[0].ToString());
                        }
                    }
                }
            }

            table.Drop();

            // Assert
            Assert.That(tables.Contains(tableName));
        }

        [Test]
        public void Should_RemoveTheTable_When_InvokeDropMethod()
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "int", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            // Act
            table.Create();

            List<string> tables = new List<string>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                string query = "SELECT name " +
                               "FROM sys.tables";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader[0].ToString());
                        }
                    }
                }
            }

            table.Drop();

            bool flag = false;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                string query = "SELECT name " +
                               "FROM sys.tables";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            flag = true;
                        }
                    }
                }
            }

            // Assert
            Assert.That(tables.Contains("Persons") && !flag);
        }
    }
}
