using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalServer.GUI.Models
{
    public class RoleBindingInformation
    {
        public RoleBindingInformation(RoleInformation roleInformation)
        {
            Id = roleInformation.Id;
            Name = roleInformation.Name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Brush BgColor { get; set; }
        public string Initials { get; set; }

        public bool EditButton { get; set; } = false;
        public bool RemoveButton { get; set; } = false;
    }
}
