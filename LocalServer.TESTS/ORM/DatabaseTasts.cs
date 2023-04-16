using LocalServer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.TESTS.ORM
{
    public class DatabaseTasts
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurityTest;Integrated Security=True;MultipleActiveResultSets=true";

        [Test]
        public void Should_ReturnDatabaseInfrastructure_When_InvokeLoadDatabaseInfrastructureMethod()
        {
            // Arrange
            Database database = new Database(connString);
            Table table = new Table("Persons", database);
            Column column = new Column("Id", "nvarchar(36)", table);
            Column column2 = new Column("Id2", "nvarchar(36)", table);
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            column2.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            Column column3 = new Column("Name", "nvarchar(64)", table);
            column3.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            table.Columns.Add(column);
            table.Columns.Add(column2);
            table.Columns.Add(column3);

            database.Tables.Add(table);

            table.Create();

            // Act

            Database database2 = new Database(connString);
            database2.LoadDatabaseInfrastructure();

            // Assert
            Assert.That(database2.Tables.First().Name == "Persons" &&
                        database2.Tables.First().Columns.First().Name == "Id" &&
                        database2.Tables.First().Columns.First().Constraints.First().Item1 == "NOT NULL" &&
                        database2.Tables.First().Columns.First().Constraints.Last().Item1 == "PRIMARY KEY");

            table.Drop();
        }
    }
}
