using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalServer.GUI.Models
{
    class DeviceBindingInformation
    {
        public DeviceBindingInformation(DeviceInformation deviceInformation)
        {
            Id = deviceInformation.Id;
            IPv4Address = deviceInformation.IPv4Address;
            Name = deviceInformation.Name;
            IsAprooved = deviceInformation.IsAprooved;
        }
        public Guid Id { get; set; }
        public string IPv4Address { get; set; }
        public string Name { get; set; }
        public bool IsAprooved { get; set; }

        public Brush BgColor { get; set; }
        public string Initials { get; set; }

        public bool EditButton { get; set; } = false;
        public bool RemoveButton { get; set; } = false;
    }
}
