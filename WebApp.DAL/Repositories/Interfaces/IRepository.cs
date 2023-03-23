using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTO;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Repositories.Interfaces
{
    public interface IRepository
    {
        public Task<IEnumerable<IResponseDataTransferObject>> GetAsync(int pagingSize, int skipAmount);
    }
}
