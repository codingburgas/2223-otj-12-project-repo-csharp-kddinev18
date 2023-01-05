using DataAccessLayer;
using LocalServerModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServerBusinessLogic
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

        public static List<UserInformation> GetUsersInformation(int pagingSize, int amount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Users").First().Select("", "", "", pagingSize, amount);
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
    }
}
