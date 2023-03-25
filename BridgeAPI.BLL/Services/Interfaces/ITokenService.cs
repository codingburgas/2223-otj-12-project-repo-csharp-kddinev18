using BridgeAPI.DAL.Models;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(IResponseDataTransferObject user);
        public Task<string> UpdateToken(IResponseDataTransferObject user, Guid tokenId);
    }
}
