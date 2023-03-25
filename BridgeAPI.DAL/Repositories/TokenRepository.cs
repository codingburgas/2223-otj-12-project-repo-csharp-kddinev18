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

        public Token AddToken(IResponseDataTransferObject user)
        {
            UserResponseDataTransferObject userObject = user as UserResponseDataTransferObject;
            Token token = new Token()
            {
                TokenId = Guid.NewGuid(),
                GlobalServerId = userObject.GlobalServerId,
                LocalServerId = userObject.LocalServerId,
                UserName = userObject.UserName,
                Role = userObject.Role,
                Email = userObject.Email,
                ExpireDate = DateTime.Now.AddHours(1),
                RenewDate = DateTime.Now.AddHours(1).AddMinutes(30)
            };
            _tokens.Add(token);
            return token;
        }

        public async Task<bool> UpdateTokenAsync(IResponseDataTransferObject user, Guid tokenId)
        {
            UserResponseDataTransferObject userObject = user as UserResponseDataTransferObject;
            Token token = await Task.Run(()=> _tokens.Where(token => token.TokenId == tokenId).FirstOrDefault());

            token.GlobalServerId = userObject.GlobalServerId;
            token.LocalServerId = userObject.LocalServerId;
            token.UserName = userObject.UserName;
            token.Role = userObject.Role;
            token.Email = userObject.Email;
            token.ExpireDate = DateTime.Now.AddHours(1);
            token.RenewDate = DateTime.Now.AddHours(1).AddMinutes(30);

            return true;
        }

        public async Task<bool> DeleteExpiredTokenAsync()
        {
            await Task.Run(() => _tokens.RemoveAll(token => token.RenewDate < DateTime.Now));
            return true;
        }

        public async Task<Token> GetTokenAsync(Guid tokenId)
        {
            return await Task.Run(() => _tokens.Where(token => token.TokenId == tokenId).FirstOrDefault());
        }
    }
}
