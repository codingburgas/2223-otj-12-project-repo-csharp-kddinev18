using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.TESTS.DataManipulation
{
    public class UserAuthenticationLogicTests
    {
        private DatabaseInitialiser _databaseInitialiser;
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurityTest;Integrated Security=True;MultipleActiveResultSets=true";

        [SetUp]
        public void SetUp()
        {
            // create a new DatabaseInitialiser object for each test
            _databaseInitialiser = new DatabaseInitialiser(20000000, connString);
            UserAuthenticationLogic.DatabaseInitialiser = _databaseInitialiser;
            _databaseInitialiser.CreateDefaultDatabaseStructure();
        }
        [TearDown]
        public void TearDown()
        {
            _databaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First().Drop();
            _databaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Drop();
            _databaseInitialiser.Database.Tables.Where(table => table.Name == "Users").First().Drop();
            _databaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First().Drop();
        }

        [Test]
        public void Should_ThrowException_When_InvokeAddAdminRoleAndThereIsAnAdmin()
        {
            // Act
            UserAuthenticationLogic.AddAdminRole();
            //  Assert
            Assert.Throws<Exception>(() => { UserAuthenticationLogic.AddAdminRole(); });
        }

        [TestCase("TestRole")]
        [TestCase("TestRole123")]
        [TestCase("123")]
        public void AddRole_ShouldAddRoleToDatabase(string roleName)
        {
            // Act
            string roleId = UserAuthenticationLogic.AddRole(roleName);

            // Assert
            Assert.NotNull(roleId);
        }

    }
}
