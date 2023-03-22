using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTO;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Repositories.Interfaces
{
    internal interface IRepository
    {
        public ICollection<IResponseDataTransferObject> Get (int pagingSize, int skipAmount, ref string errorMessage);
    }
}
