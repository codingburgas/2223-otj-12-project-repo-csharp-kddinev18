using DataAccessLayer;
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
using LocalServerBusinessLogic;

namespace LocalServerLogic
{
    public class UserAuthenticationLogic
    {
        private DatabaseIntialiser _databaseInitialiser;
        public UserAuthenticationLogic(DatabaseIntialiser databaseIntialiser)
        {
            _databaseInitialiser = databaseIntialiser;
        }
        // Retuns the hashed data using the SHA256 algorithm
        private string Hash(string data)
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
        private string GetSalt(string userName)
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
    }
}
