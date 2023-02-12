using LocalSerevr.DAL;
using LocalServer.BLL.Server.BLL;
using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.BLL.DataManipulation.BLL
{
    public static class DeviceModificationLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }

        public static List<DeviceInformation> GetDevicesInformation(int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Devices").First().Select("", "", "", pagingSize, skipAmount);
            List<DeviceInformation> devices = new List<DeviceInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                devices.Add(new DeviceInformation
                {
                    DeviceId = int.Parse(data["DeviceId"].ToString()),
                    IPv4Address = data["IPv4Address"].ToString(),
                    Name = data["Name"].ToString(),
                    IsAprooved = bool.Parse(data["IsAprooved"].ToString())
                });
            }
            return devices;
        }

        public static List<DeviceInformation> GetDevicesInformation(string name, int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Devices").First().Select("Name", "=", name, pagingSize, skipAmount);
            List<DeviceInformation> devices = new List<DeviceInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                devices.Add(new DeviceInformation
                {
                    DeviceId = int.Parse(data["DeviceId"].ToString()),
                    IPv4Address = data["IPv4Address"].ToString(),
                    Name = data["Name"].ToString(),
                    IsAprooved = bool.Parse(data["IsAprooved"].ToString())
                });
            }
            return devices;
        }

        public static int GetDevicesCount()
        {
            return DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Devices").First().GetRowsCount();
        }

        public static void EditDevice(int deviceId, string name, bool isAprooved)
        {
            Table table = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First();
            if (bool.Parse(table.Select("DeviceId", "=", deviceId.ToString()).Rows[0]["IsAprooved"].ToString()) != isAprooved)
            {
                foreach (DataRow item in DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First().Select().Rows)
                {
                    DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions").First()
                        .Insert(item["RoleId"].ToString(), deviceId.ToString(), "false", "false", "false", "false");
                }
                DatabaseInitialiser.Database.SaveDatabaseData();
            }
            table.Update("IsAprooved", isAprooved.ToString(), "DeviceId", "=", deviceId.ToString());
            table.Update("Name", name, "DeviceId", "=", deviceId.ToString());
        }

        public static void RemoveDevice(int deviceId)
        {
            DataRow dataRow = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Select("DeviceId", "=", deviceId.ToString()).Rows[0];

            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Delete("DeviceId", "=", deviceId.ToString());
        }
    }
}
