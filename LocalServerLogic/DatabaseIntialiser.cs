using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LocalServerBusinessLogic
{
    public class DatabaseIntialiser
    {
        public Database Database { get; set; }
        private static string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IOTHomeSecurity;Integrated Security=True;MultipleActiveResultSets=true";
        private long _deleteTimer;
        public DatabaseIntialiser(long deleteTimer)
        {
            _deleteTimer = deleteTimer;
            Database = new Database(_connectionString);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = deleteTimer;
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
            CreateDefaultDatabaseStructure();
        }
        public void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            foreach (Table table in Database.Tables)
            {
                if (table.Name == "Users" || table.Name == "Devices" || table.Name == "Permissions")
                    continue;

                table.Delete("Created", "<", $"DATEADD(mi,{(int)_deleteTimer / (1000 * 60)},GETDATE())", true);
            }
        }
        public void CreateDefaultDatabaseStructure()
        {
            Database.LoadDatabaseInfrastructure();
            if (!Database.Tables.Select(table => table.Name).Contains("Roles"))
            {
                Table rolesTable = new Table("Roles", Database);

                Column roleId = new Column("RoleId", "Int", rolesTable);
                roleId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                roleId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column name = new Column("Name", "nvarchar(64)", rolesTable);
                roleId.AddConstraint(new Tuple<string, object>("UNIQUE", null));
                roleId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                rolesTable.Columns.Add(roleId);
                rolesTable.Columns.Add(name);

                Database.Tables.Add(rolesTable);
                Database.SaveDatabaseInfrastructure();
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Devices"))
            {
                Table devicesTable = new Table("Devices", Database);

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
                Database.SaveDatabaseInfrastructure();
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Users"))
            {
                Table userTables = new Table("Users", Database);

                Column userId = new Column("UserId", "int", userTables);
                userId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", null));
                userId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column userName = new Column("UserName", "nvarchar(64)", userTables);
                userName.AddConstraint(new Tuple<string, object>("NOT NULL", null));
                userName.AddConstraint(new Tuple<string, object>("UNIQUE", null));

                Column email = new Column("Email", "nvarchar(128)", userTables);
                email.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column password = new Column("Password", "nvarchar(128)", userTables);
                password.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column salt = new Column("Salt", "char(15)", userTables);
                password.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column roleId = new Column("RoleId", "int", userTables);
                roleId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Roles").First().FindPrimaryKeys().First()));

                userTables.Columns.Add(userId);
                userTables.Columns.Add(userName);
                userTables.Columns.Add(email);
                userTables.Columns.Add(password);
                userTables.Columns.Add(salt);
                userTables.Columns.Add(roleId);
                Database.Tables.Add(userTables);
            }
            if (!Database.Tables.Select(table => table.Name).Contains("Permissions"))
            {
                Table permissionTable = new Table("Permissions", Database);

                Column roleId = new Column("RoleId", "int", permissionTable);
                roleId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Roles").First().FindPrimaryKeys().First()));
                roleId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "first"));

                Column deviceId = new Column("DeviceId", "int", permissionTable);
                deviceId.AddConstraint(new Tuple<string, object>("FOREIGN KEY",
                    Database.Tables.Where(table => table.Name == "Devices").First().FindPrimaryKeys().First()));
                deviceId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "second"));

                Column canCreate = new Column("CanCreate", "bit", permissionTable);
                canCreate.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column canRead = new Column("CanRead", "bit", permissionTable);
                canRead.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column canUpdate = new Column("CanUpdate", "bit", permissionTable);
                canUpdate.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                Column canDelete = new Column("CanDelete", "bit", permissionTable);
                canDelete.AddConstraint(new Tuple<string, object>("NOT NULL", null));

                permissionTable.Columns.Add(roleId);
                permissionTable.Columns.Add(deviceId);
                permissionTable.Columns.Add(canCreate);
                permissionTable.Columns.Add(canRead);
                permissionTable.Columns.Add(canUpdate);
                permissionTable.Columns.Add(canDelete);
                Database.Tables.Add(permissionTable);
            }
            Database.SaveDatabaseInfrastructure();
        }
    }
}
