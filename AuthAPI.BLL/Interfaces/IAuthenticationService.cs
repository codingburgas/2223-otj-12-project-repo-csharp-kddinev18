using AuthAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.BLL.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<IResponseDataTransferObject> LogInAsync(IRequestDataTransferObject requestObject);
        public Task<bool> RegisterAsync(IRequestDataTransferObject requestObject);
    }
}
