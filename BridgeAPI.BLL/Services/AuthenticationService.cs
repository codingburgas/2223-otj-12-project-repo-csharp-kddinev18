using BridgeAPI.DAL.Repositories;
using BridgeAPI.DAL.Repositories.Interfaces;
using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using BridgeAPI.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL
{
    public class AuthenticationService : IAuthenticationService
    {
        private IUserRepository _repository;
        public AuthenticationService() { }
        public AuthenticationService(IUserRepository repository) { _repository = repository; }

        private string Hash(IRequestDataTransferObject requestObject)
        {
            UserRequestDataTransferObject user = requestObject as UserRequestDataTransferObject;
            user.Salt = GetSalt(user.UserName);
            string data = user.Password + user.Salt;
            user.Password = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(data))).ToUpper().Replace("-", "");
            return user.Password;
        }

        private string Hash(IRequestDataTransferObject requestObject, string salt)
        {
            UserRequestDataTransferObject user = requestObject as UserRequestDataTransferObject;
            string data = user.Password + salt;
            user.Password = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(data))).ToUpper().Replace("-", "");
            return user.Password;
        }

        private static string GetSalt(string userName)
        {
            if (userName.Length <= 6)
                throw new ArgumentException("Wrong credentials");

            StringBuilder salt = new StringBuilder();
            Random random = new Random();
            salt.Append(userName.Substring(0, 6));
            for (int i = 0; i < 10; i++)
            {
                salt.Append(Convert.ToChar(random.Next(0, 26) + 65));
            }

            return salt.ToString();
        }

        public async Task<IResponseDataTransferObject> LogInAsync(IRequestDataTransferObject requestObject)
        {
            try
            {
                Hash(requestObject, await _repository.GetUserSalt(requestObject));
                return await _repository.GetUserAsync(requestObject);
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new Exception("General error");
            }
        }

        public async Task<bool> RegisterAsync(IRequestDataTransferObject requestObject)
        {
            try
            {
                Hash(requestObject);
                return await _repository.AddUserAsync(requestObject);
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new Exception("General error");
            }
        }

        public async Task<bool> CheckAuthorisation(IRequestDataTransferObject requestObject)
        {
            try
            {
                Hash(requestObject);
                return await _repository.AddUserAsync(requestObject);
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (Exception)
            {
                throw new Exception("General error");
            }
        }
    }
}
