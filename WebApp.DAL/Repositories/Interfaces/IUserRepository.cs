using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTO;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Repositories.Interfaces
{
    internal interface IUserRepository : IRepository
    {
        Task<bool> AddUserAsync (IRequestDataTransferObject user);
        Task<bool> UpdateUserAsync(int userId);
        Task<bool> DeleteUserAsync(int userId);
        Task<IResponseDataTransferObject> GetUserByIdAsync(int userId);
    }
}
