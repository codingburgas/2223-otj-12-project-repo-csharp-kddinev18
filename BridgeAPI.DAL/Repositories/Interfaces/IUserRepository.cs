using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync(IRequestDataTransferObject user);
        Task<IResponseDataTransferObject> GetUserByIdAsync(Guid userId);
        Task<IResponseDataTransferObject> GetUserAsync(IRequestDataTransferObject user);
        Task<string> GetUserSalt(IRequestDataTransferObject user);
        Task<bool> UpdateUserAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
