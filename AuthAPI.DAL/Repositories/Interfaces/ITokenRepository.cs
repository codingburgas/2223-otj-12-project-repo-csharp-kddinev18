using AuthAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.DAL.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        public Task<IResponseDataTransferObject> GetToken(IRequestDataTransferObject user);
        public Task<bool> AddToken(IRequestDataTransferObject user);
        public Task<bool> RenewToken(IRequestDataTransferObject user);
        public Task<bool> DeleteExpiredToken()
    }
}
