using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DAL.Repositories.Interfaces;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BridgeAPI.BLL
{
    public class TokenService : ITokenService
    {
        private ITokenRepository _tokenRepository;
        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }
        public string GenerateToken(IResponseDataTransferObject user)
        {
            return JsonSerializer.Serialize(_tokenRepository.AddToken(user));
        }

        public async Task<string> UpdateToken(IResponseDataTransferObject user, Guid tokenId)
        {
            return JsonSerializer.Serialize(await _tokenRepository.UpdateTokenAsync(user, tokenId));
        }
    }
}
