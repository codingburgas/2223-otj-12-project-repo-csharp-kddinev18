using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace BridgeAPI.DAL.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private static List<Token> _tokens = new List<Token>();

        public async Task<Token> AddToken(IResponseDataTransferObject user)
        {
            UserResponseDataTransferObject userObject = user as UserResponseDataTransferObject;
            Token token = await Task.Run(() => _tokens.Where(token => token.GlobalServerId == userObject.GlobalServerId).FirstOrDefault());
            if(token is null)
            {
                token = new Token()
                {
                    TokenId = Guid.NewGuid(),
                    SecretKey = GenerateSecretKey(),
                    GlobalServerId = userObject.GlobalServerId,
                    LocalServerId = userObject.LocalServerId,
                    UserName = userObject.UserName,
                    Role = userObject.Role,
                    Email = userObject.Email,
                    ExpireDate = DateTime.Now.AddHours(1),
                    RenewDate = DateTime.Now.AddHours(1).AddMinutes(30)
                };
            }
            else
            {
                await UpdateTokenAsync(userObject, token.TokenId);
            }
            _tokens.Add(token);
            return token;
        }

        private string GenerateSecretKey()
        {
            Random r = new Random();
            int itterations = 126 + r.Next(126);
            string container = "";
            for (int i = 0; i < itterations; i++)
            {
                container += (char)r.Next(127);
            }
            return container;
        }

        public async Task<Token> UpdateTokenAsync(IResponseDataTransferObject user, Guid tokenId)
        {
            UserResponseDataTransferObject userObject = user as UserResponseDataTransferObject;
            Token token = await Task.Run(()=> _tokens.Where(token => token.TokenId == tokenId).FirstOrDefault());

            if(userObject.GlobalServerId != Guid.Empty)
                token.GlobalServerId = userObject.GlobalServerId;
            if(userObject.LocalServerId != Guid.Empty)
                token.LocalServerId = userObject.LocalServerId;
            if(!string.IsNullOrEmpty(userObject.UserName))
                token.UserName = userObject.UserName;
            if(!string.IsNullOrEmpty(userObject.Role))
                token.Role = userObject.Role;
            if(!string.IsNullOrEmpty(userObject.Email))
                token.Email = userObject.Email;

            token.ExpireDate = DateTime.Now.AddHours(1);
            token.RenewDate = DateTime.Now.AddHours(1).AddMinutes(30);

            return token;
        }

        public async Task<Token> UpdateLocalServer(Guid tokenId, Guid localServerId)
        {
            Token token = await GetTokenByIdAsync(tokenId);
            token.LocalServerId = localServerId;

            return token;
        }

        public async Task<bool> DeleteExpiredTokenAsync(Guid tokenId)
        {
            await Task.Run(() => _tokens.RemoveAll(token => token.TokenId == tokenId));
            return true;
        }

        public async Task<Token> GetTokenByIdAsync(Guid tokenId)
        {
            return await Task.Run(() => _tokens.Where(token => token.TokenId == tokenId).FirstOrDefault());
        }
    }
}
