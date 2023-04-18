using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Timers;
using System.Net.Sockets;
using System.Net;
using System.Data;
using System.Security.Cryptography;

namespace LocalServer.BLL.DataManipulation.BLL
{
    public static class UserAuthenticationLogic
    {
        public static DatabaseInitialiser DatabaseInitialiser { get; set; }
        // Retuns the hashed data using the SHA256 algorithm
        private static string Hash(string data)
        {
            // Conver the output to a string and return it
            return BitConverter.ToString
                (
                    // Hashing
                    SHA256.Create().ComputeHash
                    (
                        // convert the string into bytes using UTF8 encoding
                        Encoding.UTF8.GetBytes(data)
                    )
                )
                // Convert all the characters in the string to a uppercase characters
                .ToUpper()
                // Remove the '-' from the hashed data
                .Replace("-", "");
        }
        // Generates a random sequence of characters and numbers
        private static string GetSalt(string userName)
        {
            StringBuilder salt = new StringBuilder();
            Random random = new Random();
            salt.Append(userName.Substring(0, 6));
            for (int i = 0; i < 10; i++)
            {
                // Add another character into the string builder
                salt.Append
                    (
                        // Converts the output to a char
                        Convert.ToChar
                        (
                            // Generate a random nuber between 0 and 26 and add 65 to it
                            random.Next(0, 26) + 65
                        )
                    );
            }

            // Returns the string builder's string
            return salt.ToString();
        }

        // Checks if the email is on corrent format
        private static bool CheckEmail(string email)
        {
            // Check if the email does not constains '@'
            if (email.Contains('@') == false)
                // If the email does not constains '@' it trows axception
                throw new ArgumentException("Email must contain \'@\'");

            // Return true otherwise
            return true;
        }

        // Checks if the password is on corrent format
        private static bool CheckPassword(string pass)
        {
            // Checks if the password is between 10 and 32 characters long
            if (pass.Length <= 10 || pass.Length > 32)
                throw new ArgumentException("Password must be between 10 and 32 charcters");


            // Checks if the password contains a space
            if (pass.Contains(" "))
                throw new ArgumentException("Password must not contain spaces");

            // Checks if the password doesn't conatin upper characters
            if (!pass.Any(char.IsUpper))
                throw new ArgumentException("Password must contain at least 1 upper character");

            // Checks if the password doesn't conatin lower characters
            if (!pass.Any(char.IsLower))
                throw new ArgumentException("Password must contain at least 1 lower character");

            // Checks if the password conatins upper any special symbols
            string specialCharacters = @"%!@#$%^&*()?/>.<,:;'\}]{[_~`+=-" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();
            foreach (char c in specialCharactersArray)
            {
                if (pass.Contains(c))
                    return true;
            }
            throw new ArgumentException("Password must contain at least 1 special character");
        }
        private static bool CheckUsername(string userName)
        {
            // Checks if the userName is between 6 and 64 characters long
            if (userName.Length <= 8 || userName.Length > 64)
                throw new ArgumentException("Username must be between 8 and 64 charcters");


            // Checks if the userName contains a space
            if (userName.Contains(" "))
                throw new ArgumentException("Username must not contain spaces");

            return true;
        }
        public static void AddAdminRole()
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles")
                .First().Select("Name", "=", "Admin");
            if (dataTable.Rows.Count == 0)
            {
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles").First()
                    .Insert(Guid.NewGuid().ToString(),"Admin");
                DatabaseInitialiser.Database.SaveDatabaseData();
            }
            else
            {
                throw new Exception("If want to register yourself into the platform contact your administrator.");
            }
        }
        public static string AddRole(string roleName)
        {
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles")
                .First().Insert(Guid.NewGuid().ToString(),roleName);

            DatabaseInitialiser.Database.SaveDatabaseData();

            string roleId = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles")
                .First().Select("Name", "=", roleName).Rows[0]["Id"].ToString();

            foreach (DataRow item in DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Devices").First().Select().Rows)
            {
                DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Permissions")
                    .First().Insert(roleId, item["Id"].ToString(), "false", "false", "false", "false");
            }

            DatabaseInitialiser.Database.SaveDatabaseData();

            return roleId;
        }

        public static Guid Register(string userName, string email, string password)
        {
            // Add admin role
            AddAdminRole();

            // Checks if the email is in corrent format
            CheckUsername(userName);
            // Checks if the email is in corrent format
            CheckEmail(email);
            // Checks if the password is in corrent format
            CheckPassword(password);
            // Gets the salt
            string salt = GetSalt(userName);
            // Hashes the password combinded with the salt
            string hashPassword = Hash(password + salt);
            Guid adminRole = new Guid(DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles")
                .First().Select("Name", "=", "Admin").Rows[0]["Id"].ToString());
            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Insert(Guid.NewGuid().ToString(), userName, email, hashPassword, salt, adminRole.ToString());
            DatabaseInitialiser.Database.SaveDatabaseData();

            DataTable dataTable = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Select("UserName", "=", userName);

            return new Guid(dataTable.Rows[0]["Id"].ToString());
        }

        public static void RegisterMember(string userName, string email, string password, string roleName)
        {
            string roleId = string.Empty;
            try
            {
                roleId = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles")
                    .First().Select("Name", "=", roleName).Rows[0]["Id"].ToString();
            }
            catch (Exception)
            {
                roleId = AddRole(roleName);
            }
            // Checks if the email is in corrent format
            CheckUsername(userName);
            // Checks if the email is in corrent format
            CheckEmail(email);
            // Checks if the password is in corrent format
            CheckPassword(password);
            // Gets the salt
            string salt = GetSalt(userName);
            // Hashes the password combinded with the salt
            string hashPassword = Hash(password + salt);

            DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Insert(Guid.NewGuid().ToString(), userName, email, hashPassword, salt, roleId);
            DatabaseInitialiser.Database.SaveDatabaseData();
        }

        public static Guid LogIn(string userName, string password)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Select("UserName", "=", userName);
            if (dataTable.Rows.Count == 0)
            {
                throw new Exception("Wrong credentials");
            }
            string hashPassword = Hash(password + dataTable.Rows[0]["Salt"].ToString());
            if (hashPassword != dataTable.Rows[0]["Password"].ToString())
            {
                throw new Exception("Wrong credentials");
            }

            return new Guid (DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Select("UserName", "=", userName).Rows[0]["Id"].ToString());
        }

        public static bool IsAdmin(Guid userId)
        {
            DataTable dataTable = DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Users")
                .First().Select("Id", "=", userId.ToString());

            return DatabaseInitialiser.Database.Tables.Where(table => table.Name == "Roles")
                .First().Select("Id", "=", dataTable.Rows[0]["RoleId"].ToString()).Rows[0]["Name"].ToString() == "Admin";
        }
    }
}
