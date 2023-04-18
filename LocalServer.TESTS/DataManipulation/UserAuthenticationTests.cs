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

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_RegisterUser_When_InvokeRegisterMethod(string userName, string email, string password)
        {
            // Act
            Guid userId = UserAuthenticationLogic.Register(userName, email, password);

            // Assert
            Assert.NotNull(userId);
        }

        [TestCase("asd", "kddinev18", "Passwo")]
        [TestCase("", "", "")]
        [TestCase("hev", "MilkoMilchev.bg", "MilkoMilch")]
        public void Should_ThrowException_When_InvokeRegisterMethodWithIncorrectData(string userName, string email, string password)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => { UserAuthenticationLogic.Register(userName, email, password); });
        }

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_GetUserId_When_InvokeLogInMethod(string userName, string email, string password)
        {
            // Arrange
            Guid userId = UserAuthenticationLogic.Register(userName, email, password);

            // Act
            Guid newUserId = UserAuthenticationLogic.LogIn(userName, password);

            // Assert
            Assert.AreEqual(userId, newUserId);
        }

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_ThrowException_When_InvokeLogInMethodWithWringCredentials(string userName, string email, string password)
        {
            // Arrange
            UserAuthenticationLogic.Register(userName, email, password);

            // Act
            UserAuthenticationLogic.LogIn(userName, password);

            // Assert
            Assert.Throws<Exception>(() => { UserAuthenticationLogic.LogIn(userName+"asdasdasdasdas", password); });
        }
    }
}
