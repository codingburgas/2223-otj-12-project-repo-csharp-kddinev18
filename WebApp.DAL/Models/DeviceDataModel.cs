using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class DeviceDataModel
    {
        public List<string> Infrastructure { get; set; }
        public List<List<string>> Data { get; set; }
    }
}
