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
    public static class RoleModificationLogic
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
                    Id = new Guid(data["RoleId"].ToString()),
                    Name = data["Name"].ToString()
                });
            }
            return roles;
        }

        public static List<RoleInformation> GetRolesInformation(string roleName, int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Roles").First().Select("Name", "=", roleName, pagingSize, skipAmount);
            List<RoleInformation> roles = new List<RoleInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                roles.Add(new RoleInformation()
                {
                    Id = new Guid(data["RoleId"].ToString()),
                    Name = data["Name"].ToString()
                });
            }
            return roles;
        }

        public static int GetRolesCount()
        {
            return DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Roles").First().GetRowsCount();
        }

        public static void EditRole(Guid roleId, string roleName)
        {
            Table table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First();
            table.Update("Name", roleName, "Id", "=", roleId.ToString());
        }

        public static void RemoveRole(Guid roleId)
        {
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First()
                .Delete("Id", "=", roleId.ToString());
        }

        public static void AddRole(string name)
        {
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First().Insert(name);
            DatabaseInitialiser.Database.SaveDatabaseData();
            int roleId = int.Parse(DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First()
                .Select("Name", "=", name).Rows[0]["Id"].ToString());

            foreach (DataRow item in DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Select().Rows)
            {
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First()
                    .Insert(roleId.ToString(), item["DeviceId"].ToString(), "false", "false", "false", "false");
            }
            DatabaseInitialiser.Database.SaveDatabaseData();
        }
    }
}
