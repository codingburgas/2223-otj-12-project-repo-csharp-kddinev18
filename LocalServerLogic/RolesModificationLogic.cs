using LocalServerModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServerBusinessLogic
{
    public static class RolesModificationLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }
        public static List<RoleInformation> GetRolesInformation(int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Roles").First().Select("", "", "", pagingSize, skipAmount);
            List<RoleInformation> roles = new List<RoleInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                roles.Add(new RoleInformation()
                {
                    RoleId = int.Parse(data["RoleId"].ToString()),
                    Name = data["Name"].ToString()
                });
            }
            return roles;
        }
    }
}
