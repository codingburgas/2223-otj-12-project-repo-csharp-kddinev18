using LocalServerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalServerGUI.Models
{
    public class RoleBindingInformation
    {
        public RoleBindingInformation(RoleInformation roleInformation)
        {
            RoleId = roleInformation.RoleId;
            Name = roleInformation.Name;
        }
        public int RoleId { get; set; }
        public string Name { get; set; }

        public Brush BgColor { get; set; }
        public string Initials { get; set; }

        public bool EditButton { get; set; } = false;
        public bool RemoveButton { get; set; } = false;
    }
}
