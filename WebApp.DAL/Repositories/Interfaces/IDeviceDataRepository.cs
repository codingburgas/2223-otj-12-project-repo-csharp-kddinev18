using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Repositories.Interfaces
{
    public interface IDeviceDataRepository
    {
        public IEnumerable<IResponseDataTransferObject> Get(DateTime from, DateTime to, int pagingSize, int skipAmount);
    }
}
