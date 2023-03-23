using LocalServer.DAL;
using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.BLL.DataManipulation.BLL
{
    public static class UserModifierLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }

        public static UserInformation GetCurrentUserInformation(int userId)
        {
            DataRow dataRow = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Select("UserId", "=", userId.ToString()).Rows[0];
            return new UserInformation()
            {
                UserId = userId,
                UserName = dataRow["UserName"].ToString(),
                Email = dataRow["Email"].ToString(),
                Role = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Roles").First()
                .Select("RoleId", "=", dataRow["RoleId"].ToString()).Rows[0]["Name"].ToString()
            };
        }

        public static List<UserInformation> GetUsersInformation(int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Users").First().Select("", "", "", pagingSize, skipAmount);
            List<UserInformation> users = new List<UserInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                users.Add(new UserInformation()
                {
                    UserId = int.Parse(data["UserId"].ToString()),
                    Email = data["Email"].ToString(),
                    UserName = data["UserName"].ToString(),
                    Role = DatabaseInitialiser.Database.Tables
                    .Where(table => table.Name == "Roles").First()
                    .Select("RoleId", "=", data["RoleId"].ToString()).Rows[0]["Name"].ToString()
                });
            }
            return users;
        }

        public static List<UserInformation> GetUsersInformation(string userName, int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Users").First().Select("UserName", "=", userName, pagingSize, skipAmount);
            List<UserInformation> users = new List<UserInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                users.Add(new UserInformation()
                {
                    UserId = int.Parse(data["UserId"].ToString()),
                    Email = data["Email"].ToString(),
                    UserName = data["UserName"].ToString(),
                    Role = DatabaseInitialiser.Database.Tables
                    .Where(table => table.Name == "Roles").First()
                    .Select("RoleId", "=", data["RoleId"].ToString()).Rows[0]["Name"].ToString()
                });
            }
            return users;
        }

        public static int GetUsersCount()
        {
            return DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Users").First().GetRowsCount();
        }

        public static void EditUser(int userId, string userName, string email, string role)
        {
            Table table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users").First();
            table.Update("UserName", userName, "UserId", "=", userId.ToString());
            table.Update("Email", email, "UserId", "=", userId.ToString());
            string roleId = string.Empty;
            try
            {
                roleId = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First()
                    .Select("Name", "=", role).Rows[0]["RoleId"].ToString();
            }
            catch (Exception)
            {
                roleId = UserAuthenticationLogic.AddRole(role);
            }

            table.Update("RoleId", roleId.ToString(), "UserId", "=", userId.ToString());
        }

        public static void RemoveUser(int userId)
        {
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users").First()
                .Delete("UserId", "=", userId.ToString());
        }
    }
}
