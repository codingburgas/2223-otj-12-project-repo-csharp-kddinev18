using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.TESTS.DataManipulation
{
    public class RoleModificationLogicTest
    {
        private DatabaseInitialiser _databaseInitialiser;
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurityTest;Integrated Security=True;MultipleActiveResultSets=true";

        [SetUp]
        public void SetUp()
        {
            // create a new DatabaseInitialiser object for each test
            _databaseInitialiser = new DatabaseInitialiser(20000000, connString);
            UserAuthenticationLogic.DatabaseInitialiser = _databaseInitialiser;
            RoleModificationLogic.DatabaseInitialiser = _databaseInitialiser;
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

        [TestCase("User")]
        [TestCase("User123123")]
        [TestCase("123123")]
        public void Should_CreateRole_When_IvokeCreateRoleMethod(string roleName)
        {
            // Arrange
            RoleModificationLogic.AddRole(roleName);
            // Act
            int count = RoleModificationLogic.GetRolesCount();
            // Assert
            Assert.AreEqual(count, 1);
        }

        [TestCase("User")]
        [TestCase("User123123")]
        [TestCase("123123")]
        public void Should_ReturnRolesInformation_When_IvokeGetRolesInformationMethod(string roleName)
        {
            // Arrange
            RoleModificationLogic.AddRole(roleName);
            // Act
            List<RoleInformation> roles = RoleModificationLogic.GetRolesInformation(10, 0);
            // Assert
            Assert.AreEqual(roles.Count(), 1);
            Assert.AreEqual(roles.First().Name, roleName);
        }

        [TestCase("User")]
        [TestCase("User123123")]
        [TestCase("123123")]
        public void Should_ReturnFilteredRolesInformation_When_IvokeGetRolesInformationMethod(string roleName)
        {
            // Arrange
            RoleModificationLogic.AddRole(roleName);
            // Act
            List<RoleInformation> roles = RoleModificationLogic.GetRolesInformation(roleName, 10, 0);
            // Assert
            Assert.AreEqual(roles.Count(), 1);
            Assert.AreEqual(roles.First().Name, roleName);
        }
    }
}
