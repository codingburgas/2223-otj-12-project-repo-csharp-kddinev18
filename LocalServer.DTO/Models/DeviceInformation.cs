using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.DTO.Models
{
    public class DeviceInformation
    {
        public int DeviceId { get; set; }
        public string IPv4Address { get; set; }
        public string Name { get; set; }
        public bool IsAprooved { get; set; }
    }
}
