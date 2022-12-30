using DataAccessLayer;
using LocalServerLogic;

namespace LocalServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*ServerLogic server = new ServerLogic(5400);
            server.ServerSetUp();
            Console.ReadKey();
            server.ServerShutDown();*/
            Database database = new Database(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ORMTest;Integrated Security=True;MultipleActiveResultSets=true");
            database.LoadDatabaseInfrastructure();
            Table userTables = new Table("Users");

            Column userId = new Column("UserId", "int", userTables);
            userId.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "first"));
            userId.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            Column userId2 = new Column("UserId2", "int", userTables);
            userId2.AddConstraint(new Tuple<string, object>("PRIMARY KEY", "second"));
            userId2.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            Column userName = new Column("UserName", "nvarchar(64)", userTables);
            userName.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            Column email = new Column("Email", "nvarchar(128)", userTables);
            email.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            Column password = new Column("Password", "nvarchar(128)", userTables);
            password.AddConstraint(new Tuple<string, object>("NOT NULL", null));

            userTables.Columns.Add(userId);
            userTables.Columns.Add(userId2);
            userTables.Columns.Add(userName);
            userTables.Columns.Add(email);
            userTables.Columns.Add(password);
            Database.Tables.Add(userTables);
            database.SaveDatabaseInfrastructure();
        }
    }
}