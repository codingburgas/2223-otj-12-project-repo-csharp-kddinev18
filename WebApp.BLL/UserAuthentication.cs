using System.Data;
using System.Security.Cryptography;
using System.Text;
using WebApp.DAL;

namespace WebApp.BLL
{
    public static class UserAuthentication
    {
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

        public static void Register(User user, IOTHomeSecurityDbContext dbContext)
        {
            // Checks if the email is in corrent format
            CheckUsername(user.UserName);
            // Checks if the email is in corrent format
            CheckEmail(user.Email);
            // Checks if the password is in corrent format
            CheckPassword(user.Password);
            // Gets the salt
            string salt = GetSalt(user.UserName);
            // Hashes the password combinded with the salt
            string hashPassword = Hash(user.Password + salt);

            user.Password = hashPassword;
            user.Salt = salt;
            user.DateRegisterd = DateTime.Now;

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}