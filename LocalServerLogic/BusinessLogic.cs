using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServerLogic
{
    public class BusinessLogic
    {
        private void CreateDefaultDatabaseStructure(Database database)
        {
            database.LoadDatabaseInfrastructure();
            if (!Database.Tables.Select(table => table.Name).Contains("Devices"))
            {
                Table devicesTable = new Table("Devices");

                Column deviceId = new Column("DeviceId", "int", devicesTable);
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                deviceId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column ipv4Address = new Column("IPv4Address", "nvarchar(64)", devicesTable);
                ipv4Address.AddConstraint(new Tuple<string, object>("NOT NULL", null));
                ipv4Address.AddConstraint(new Tuple<string, object>("UNIQUE", null));

                Column name = new Column("Name", "nvarchar(64)", devicesTable);
                name.AddConstraint(new Tuple<string, object>("NOT NULL", null));
                name.AddConstraint(new Tuple<string, object>("UNIQUE", null));

                Column aprooved = new Column("IsAprooved", "bit", devicesTable);
                aprooved.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                devicesTable.Columns.Add(deviceId);
                devicesTable.Columns.Add(ipv4Address);
                devicesTable.Columns.Add(name);
                devicesTable.Columns.Add(aprooved);
                Database.Tables.Add(devicesTable);
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Users"))
            {
                Table userTables = new Table("Users");

                Column userId = new Column("UserId", "int", userTables);
                userId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                userId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column userName = new Column("UserName", "nvarchar(64)", userTables);
                userName.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column email = new Column("Email", "nvarchar(128)", userTables);
                email.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column password = new Column("Password", "nvarchar(128)", userTables);
                password.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                userTables.Columns.Add(userId);
                userTables.Columns.Add(userName);
                userTables.Columns.Add(email);
                userTables.Columns.Add(password);
                Database.Tables.Add(userTables);
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Permissions"))
            {
                Table permissionTables = new Table("Permissions");

                Column userId = new Column("UserId", "int", permissionTables);
                userId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Users").First().Columns.Where(column => column.Name == "UserId").First()));

                userId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "first"));
                userId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column deviceId = new Column("DeviceId", "int", permissionTables);
                deviceId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Devices").First().Columns.Where(column => column.Name == "DeviceId").First()));
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "second"));
                deviceId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                permissionTables.Columns.Add(userId);
                permissionTables.Columns.Add(deviceId);
                Database.Tables.Add(permissionTables);
            }
            database.SaveDatabaseInfrastructure();
        }
    }
}
