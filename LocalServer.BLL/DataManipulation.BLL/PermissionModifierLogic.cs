using LocalSerevr.DAL;
using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.BLL.DataManipulation.BLL
{
    public static class PermissionModifierLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }

        public static List<PermissionInformation> GetPermissionInformation(int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Permissions").First().Select("", "", "", pagingSize, skipAmount);
            List<PermissionInformation> permissions = new List<PermissionInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                permissions.Add(new PermissionInformation()
                {
                    RoleName = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First()
                    .Select("RoleId", "=", data["RoleId"].ToString()).Rows[0]["Name"].ToString(),

                    DeviceName = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                    .Select("DeviceId", "=", data["DeviceId"].ToString()).Rows[0]["Name"].ToString(),

                    CanCreate = bool.Parse(data["CanCreate"].ToString()),
                    CanRead = bool.Parse(data["CanRead"].ToString()),
                    CanUpdate = bool.Parse(data["CanUpdate"].ToString()),
                    CanDelete = bool.Parse(data["CanDelete"].ToString())
                });
            }
            permissions.OrderBy(permission => permission.RoleName);
            return permissions;
        }

        public static int GetPermissionsCount()
        {
            return DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Permissions").First().GetRowsCount();
        }

        public static void EditPermission(string roleName, string deviceName, bool create, bool read, bool update, bool delete)
        {
            int roleId = int.Parse(DatabaseInitialiser.Database.Tables.Where(Table => Table.Name == "Roles").First().Select("Name", "=", roleName).Rows[0]["RoleId"].ToString());
            int deviceId = int.Parse(DatabaseInitialiser.Database.Tables.Where(Table => Table.Name == "Devices").First().Select("Name", "=", deviceName).Rows[0]["DeviceId"].ToString());

            Table table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First();
            table.Update("CanCreate", create.ToString(), "", "", "", true, $"RoleId = {roleId} AND DeviceId = {deviceId}");
            table.Update("CanRead", read.ToString(), "", "", "", true, $"RoleId = {roleId} AND DeviceId = {deviceId}");
            table.Update("CanUpdate", update.ToString(), "", "", "", true, $"RoleId = {roleId} AND DeviceId = {deviceId}");
            table.Update("CanDelete", delete.ToString(), "", "", "", true, $"RoleId = {roleId} AND DeviceId = {deviceId}");
        }

        public static void RemovePermission(string roleName, string deviceName)
        {
            int roleId = int.Parse(DatabaseInitialiser.Database.Tables.Where(Table => Table.Name == "Roles").First().Select("Name", "=", roleName).Rows[0]["RoleId"].ToString());
            int deviceId = int.Parse(DatabaseInitialiser.Database.Tables.Where(Table => Table.Name == "Devices").First().Select("Name", "=", deviceName).Rows[0]["DeviceId"].ToString());

            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First()
                .Delete("", "", "", true, $"RoleId = {roleId} AND DeviceId = {deviceId}");
        }
    }
}
