using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTO;

namespace WebApp.DAL.Repositories.Interfaces
{
    internal interface IRepository
    {
        public ICollection<IResponseDataTranferObject> Get (int pagingSize, int skipAmount);
    }
}
