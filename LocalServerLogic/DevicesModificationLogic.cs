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
    public static class DevicesModificationLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }

        public static List<DevicesInformation> GetDevices(int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Devices").First().Select("", "", "", pagingSize, skipAmount);
            List<DevicesInformation> devices = new List<DevicesInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                devices.Add(new DevicesInformation { 
                    DeviceId = int.Parse(data["DeviceId"].ToString()),
                    IPv4Address = data["IPv4Address"].ToString(),
                    Name = data["Name"].ToString(),
                    IsAprooved = bool.Parse(data["IsAprooved"].ToString())
                });
            }
            return devices;
        }

        public static List<DevicesInformation> GetDevices(string name ,int pagingSize, int skipAmount)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables
                .Where(table => table.Name == "Devices").First().Select("Name", "=", name, pagingSize, skipAmount);
            List<DevicesInformation> devices = new List<DevicesInformation>();
            foreach (DataRow data in dataTable.Rows)
            {
                devices.Add(new DevicesInformation
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
    }
}
