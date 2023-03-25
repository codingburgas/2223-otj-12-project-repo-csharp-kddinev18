using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DAL.Models;
using BridgeAPI.DAL.Repositories.Interfaces;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public Token GenerateToken(IResponseDataTransferObject user)
        {
            return _tokenRepository.AddToken(user);
        }

        public async Task<Token> UpdateToken(IResponseDataTransferObject user)
        {
            return await _tokenRepository.UpdateTokenAsync()
        }
    }
}
