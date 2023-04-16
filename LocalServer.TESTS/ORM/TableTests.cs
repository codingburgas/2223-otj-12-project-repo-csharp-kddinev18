using LocalServer.DAL;
using System;
using System.Collections.Generic;
using System.Data;
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


            // Assert
            Assert.That(tables.Contains(tableName));


            table.Drop();
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

            // Act
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

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_ReturnDataTable_When_InvokeSelectMethod(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();
            // Act

            DataTable dataTable = table.Select("Id", "=", guidString);

            // Assert
            Assert.AreEqual(dataTable.Rows[0]["Name"].ToString(), personName);

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_ThrowException_When_InvokeSelectMethodWithIncorrectColumnName(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();

            // Act & Assert
            Assert.Throws<Exception>(() => table.Select("Id321", "=", guidString));

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_ThrowException_When_InvokeSelectMethodWithIncorrectOperation(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();

            // Act & Assert
            Assert.Throws<Exception>(() => table.Select("Id", "asdashtf", guidString));

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_ReturnRowCount_When_InvokeGetRowsCountMethod(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();
            // Act

            int count = table.GetRowsCount();

            // Assert
            Assert.AreEqual(count, 1);

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_DeleteRow_When_InvokeDeleteMethod(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();
            int countBeforeDeletion = table.GetRowsCount();
            // Act

            table.Delete("Id", "=", guidString);
            int countAfterDeletion = table.GetRowsCount();

            // Assert
            Assert.AreNotEqual(countAfterDeletion, countBeforeDeletion);

            table.Drop();
        }


        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_ThrowException_When_InvokeDeleteMethodWithIncorrectColumnName(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();
            int countBeforeDeletion = table.GetRowsCount();

            // Act & Assert
            Assert.Throws<Exception>(() => table.Delete("Id13123123", "=", guidString));

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_ThrowException_When_InvokeDeleteMethodWithIncorrectOperation(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();
            int countBeforeDeletion = table.GetRowsCount();

            // Act & Assert
            Assert.Throws<Exception>(() => table.Delete("Id", "123123123", guidString));

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", "Names")]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", "Names1")]
        [TestCase("a3dd17d3-b648-4367-90ce-f316c9c23d93", "123")]
        public void Should_InsertRow_When_InvokeInsertMethod(string guidString, string personName)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();
            // Act

            table.Insert(new Guid(guidString).ToString(), personName);
            database.SaveDatabaseData();

            int countAfterInsertion = table.GetRowsCount();

            // Assert
            Assert.AreEqual(countAfterInsertion, 1);

            table.Drop();
        }

        [TestCase("d68473e7-7e8a-423e-9306-97d96e3ae8e0", new string[] { "d68473e7-7e8a-423e-9306-97d96e3ae8e0", "asdasdasd", "asdasdasd"})]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", new string[] { "d68473e7-7e8a-423e-9306-97d96e3ae8e0" })]
        [TestCase("d6102451-3251-4525-9a6b-78dacda1fb3c", new string[] { "a" })]
        public void Should_ThrowException_When_InvokeInsertMethodWithIcorretData(string guidString, string[] data)
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column2 = new Column("Name", "nvarchar(64)", table);
            column2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);

            database.Tables.Add(table);

            table.Create();

            // Act
            table.Insert(data);
            // Assert
            Assert.Throws<SqlException>(() => database.SaveDatabaseData());

            table.Drop();
        }
    }
}
