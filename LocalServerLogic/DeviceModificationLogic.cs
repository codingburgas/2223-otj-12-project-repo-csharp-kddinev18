using DataAccessLayer;
using LocalServerModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LocalServerBusinessLogic
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
                devices.Add(new DeviceInformation { 
                    DeviceId = int.Parse(data["DeviceId"].ToString()),
                    IPv4Address = data["IPv4Address"].ToString(),
                    Name = data["Name"].ToString(),
                    IsAprooved = bool.Parse(data["IsAprooved"].ToString())
                });
            }
            return devices;
        }

        public static List<DeviceInformation> GetDevicesInformation(string name ,int pagingSize, int skipAmount)
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

            table.Update("Name", name, "DeviceId", "=", deviceId.ToString());
            table.Update("IsAprooved", isAprooved.ToString(), "DeviceId", "=", deviceId.ToString());
        }

        public static void RemoveDevice(int deviceId)
        {
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First()
                .Delete("DeviceId", "=", deviceId.ToString());
        }
    }
}
