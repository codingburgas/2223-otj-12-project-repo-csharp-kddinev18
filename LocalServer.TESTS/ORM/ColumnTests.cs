using LocalServer.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.TESTS.ORM
{
    public class ColumnTests
    {
        [Test]
        public void Should_ReturnSQLCode_When_InvokingToString()
        {
            // Arrange
            Column column = new Column("Id", "int", new Table("Table", new Database()));
            column.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
            column.AddConstraint(new Tuple<string, object>("NOT NULL", null));
            // Act
            string container = column.ToString();
            // Assert
            Assert.AreEqual(container, "[Id] int  NOT NULL ,\n");
        }
    }
}
