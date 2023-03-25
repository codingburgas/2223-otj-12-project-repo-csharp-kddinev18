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
        public bool GenerateToken(IResponseDataTransferObject user);
        public bool UpdateToken(IResponseDataTransferObject user);
        public bool RenewToken(IResponseDataTransferObject user);
    }
}
