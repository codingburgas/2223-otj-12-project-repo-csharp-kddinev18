using LocalServer.BLL.DataManipulation.BLL;
using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.TESTS.DataManipulation
{
    public class UserManipulationLogicTest
    {
        private DatabaseInitialiser _databaseInitialiser;
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurityTest;Integrated Security=True;MultipleActiveResultSets=true";

        [SetUp]
        public void SetUp()
        {
            // create a new DatabaseInitialiser object for each test
            _databaseInitialiser = new DatabaseInitialiser(20000000, connString);
            UserAuthenticationLogic.DatabaseInitialiser = _databaseInitialiser;
            UserModifierLogic.DatabaseInitialiser = _databaseInitialiser;
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

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_ReturnTheCurrentUserInformation_When_IvokeGetCurrentUserInformationMethod(string userName, string email, string password)
        {
            // Arrange
            Guid id = UserAuthenticationLogic.Register(userName, email, password);
            // Act
            UserInformation user = UserModifierLogic.GetCurrentUserInformation(id);
            // Assert
            Assert.AreEqual(user.UserName, userName);
            Assert.AreEqual(user.Email, email);
            Assert.AreEqual(user.Id, id);
        }

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_ReturnUsersInformation_When_IvokeGetUsersInformationMethod(string userName, string email, string password)
        {
            // Arrange
            UserAuthenticationLogic.Register(userName, email, password);
            UserAuthenticationLogic.RegisterMember(userName + "1", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "2", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "3", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "4", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "5", email, password, "user");
            // Act
            List<UserInformation> user = UserModifierLogic.GetUsersInformation(10,0);
            // Assert
            Assert.AreEqual(user.Count(), 6);
            Assert.That(user.Select(u => u.UserName).Contains(userName + "3"));
        }

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_ReturnFilteredUsersInformation_When_IvokeGetUsersInformationMethod(string userName, string email, string password)
        {
            // Arrange
            UserAuthenticationLogic.Register(userName, email, password);
            UserAuthenticationLogic.RegisterMember(userName + "1", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "2", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "3", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "4", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "5", email, password, "user");
            // Act
            List<UserInformation> user = UserModifierLogic.GetUsersInformation(userName + "1", 10, 0);
            // Assert
            Assert.AreEqual(user.Count(), 1);
            Assert.That(user.Select(u => u.UserName).Contains(userName + "1"));
        }

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_ReturnUsersCount_When_IvokeGetUsersCountMethod(string userName, string email, string password)
        {
            // Arrange
            UserAuthenticationLogic.Register(userName, email, password);
            UserAuthenticationLogic.RegisterMember(userName + "1", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "2", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "3", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "4", email, password, "user");
            UserAuthenticationLogic.RegisterMember(userName + "5", email, password, "user");
            // Act
            int count = UserModifierLogic.GetUsersCount();
            // Assert
            Assert.AreEqual(count, 6);
        }

        [TestCase("kddinev18", "kddinev18@abv.bg", "Password1!1!")]
        [TestCase("TATinevich", "TATinevich@as", "sdf^%$fgDtrw543SDF")]
        [TestCase("MilkoMilchev", "MilkoMilchev@abv.bg", "MilkoMilchev234#$")]
        public void Should_EditUser_When_IvokeEditUserMethod(string userName, string email, string password)
          {
            // Arrange
            Guid id = UserAuthenticationLogic.Register(userName, email, password);
            // Act
            UserModifierLogic.RemoveUser(id);
            // Assert
            Assert.Throws<IndexOutOfRangeException>(() => { UserModifierLogic.GetCurrentUserInformation(id); });
        }
    }
}
