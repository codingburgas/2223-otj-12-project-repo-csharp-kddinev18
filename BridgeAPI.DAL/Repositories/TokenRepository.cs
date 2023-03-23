using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.DAL.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private static List<Token> tokens = new List<Token>();

        public Task<bool> AddToken(IRequestDataTransferObject user)
        {
            UserRequestDataTrasferObject userObject = user as UserRequestDataTrasferObject;
            tokens.Add(new Token()
            {
                Id = Guid.NewGuid(),
                UserName = userObject.UserName,
                Email = userObject.Email,
                //Role = "Test"
            });
            throw new NotImplementedException();

        }

        public Task<bool> DeleteExpiredToken()
        {
            throw new NotImplementedException();
        }

        public Task<IResponseDataTransferObject> GetToken(IRequestDataTransferObject user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RenewToken(IRequestDataTransferObject user)
        {
            throw new NotImplementedException();
        }
    }
}
