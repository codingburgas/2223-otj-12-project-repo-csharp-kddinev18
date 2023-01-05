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
    public class UserModifierLogic
    {
        private DatabaseInitialiser _databaseIntialiser;
        public UserModifierLogic(DatabaseInitialiser databaseIntialiser)
        {
            _databaseIntialiser = databaseIntialiser;
        }

        public UserInformation GetCurrentUserInformation(int userId)
        {
            DataRow dataRow = _databaseIntialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Select("UserId", "=", userId.ToString()).Rows[0];
            return new UserInformation()
            {
                UserName = dataRow["UserName"].ToString(),
                Email = dataRow["Email"].ToString()
            };
        }

        public List<UserInformation> GetUsers(int pagingSize, int amount)
        {
            DataTable dataTable = _databaseIntialiser.Database.Tables
                .Where(table => table.Name == "Users").First().Select("", "", "", pagingSize, amount);
            List<UserInformation> users = new List<UserInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                users.Add(new UserInformation()
                {
                    Email = data["Email"].ToString(),
                    UserName = data["UserName"].ToString()
                });
            }
            return users;
        }

        public int GetUsersCount()
        {
            return _databaseIntialiser.Database.Tables
                .Where(table => table.Name == "Users").First().GetRowsCount();
        }
    }
}
