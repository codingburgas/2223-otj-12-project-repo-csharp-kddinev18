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
        bool AddUser (IRequestDataTransferObject user, ref string errorMessage);
        bool UpdateUser (int userId, ref string errorMessage);
        bool DeleteUser (int userId, ref string errorMessage);
        IResponseDataTransferObject GetUserById (int userId, ref string errorMessage);
    }
}
