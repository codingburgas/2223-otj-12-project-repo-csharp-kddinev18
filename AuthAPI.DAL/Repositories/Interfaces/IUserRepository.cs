using AuthAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> AddUserAsync (IRequestDataTransferObject user);
        Task<IResponseDataTransferObject> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserAsync(Guid userId);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
