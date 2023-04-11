using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DAL.Repositories.Interfaces;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BridgeAPI.BLL
{
    public class TokenService : ITokenService
    {
        private ITokenRepository _repository;
        public TokenService(ITokenRepository tokenRepository)
        {
            _repository = tokenRepository;
        }
        public async Task<string> GenerateToken(IResponseDataTransferObject user)
        {
            return JsonSerializer.Serialize(await _repository.AddToken(user));
        }

        public async Task<Token> GenerateTokenType(IResponseDataTransferObject user)
        {
            return await _repository.AddToken(user);
        }

        public async Task<Token> GetToken(Guid tokenId)
        {
            return await _repository.GetTokenByIdAsync(tokenId);
        }

        public async Task<string> UpdateToken(IResponseDataTransferObject user, Guid tokenId)
        {
            return JsonSerializer.Serialize(await _repository.UpdateTokenAsync(user, tokenId));
        }

        public async Task<string> UpdateLocalServer(Guid tokenId, Guid localServerId)
        {
            return JsonSerializer.Serialize(await _repository.UpdateLocalServer(tokenId, localServerId));
        }

        public void DeleteToken(Guid tokenId)
        {
            _repository.DeleteExpiredTokenAsync(tokenId);
        }

        public async Task<Token> CeckAuthentication(JsonObject jObject, bool localServerAuthentication = false)
        {
            Token userToken = JsonSerializer.Deserialize<Token>(jObject["Token"].ToString());
            Token serverToken = await GetToken(userToken.TokenId);
            if (serverToken is null)
            {
                throw new UnauthorizedAccessException("Not authenticated");
            }
            if (serverToken.ExpireDate < DateTime.Now)
            {
                await _repository.DeleteExpiredTokenAsync(serverToken.TokenId);
                throw new UnauthorizedAccessException("Token is expired");
            }
            if (serverToken.SecretKey != userToken.SecretKey)
            {
                throw new UnauthorizedAccessException("Not authenticated");
            }
            if (localServerAuthentication)
            {
                if (userToken.LocalServerId == Guid.Empty)
                {
                    throw new UnauthorizedAccessException("Not authenticated");
                }
            }
            return userToken;
        }
    }
}
