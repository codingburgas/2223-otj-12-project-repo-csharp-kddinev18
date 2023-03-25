using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DAL.Models;
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
        public bool GenerateToken(IResponseDataTransferObject user)
        {

            throw new NotImplementedException();
        }

        public bool RenewToken(IResponseDataTransferObject user)
        {
            throw new NotImplementedException();
        }

        public bool UpdateToken(IResponseDataTransferObject user)
        {
            throw new NotImplementedException();
        }
    }
}
