using BridgeAPI.DAL.Models;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.DAL.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        public Task<Token> GetTokenByIdAsync(Guid tokenId);
        public Task<Token> AddToken(IResponseDataTransferObject user);
        public Task<Token> UpdateTokenAsync(IResponseDataTransferObject user, Guid tokenId);
        public Task<Token> UpdateLocalServer(Guid tokenId, Guid localServerId);
        public Task<bool> DeleteExpiredTokenAsync(Guid tokenId);
    }
}
