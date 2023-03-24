using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<IResponseDataTransferObject> LogInAsync(IRequestDataTransferObject requestObject);
        public Task<IResponseDataTransferObject> LogInLocalServerAsync(IRequestDataTransferObject requestObject);
        public Task<bool> RegisterAsync(IRequestDataTransferObject requestObject);
    }
}
