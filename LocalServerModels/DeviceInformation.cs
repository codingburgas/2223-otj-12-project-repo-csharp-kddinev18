using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServerModels
{
    public class DeviceInformation
    {
        public int DeviceId { get; set; }
        public string IPv4Address { get; set; }
        public string Name { get; set; }
        public bool IsAprooved { get; set; }
    }
}
