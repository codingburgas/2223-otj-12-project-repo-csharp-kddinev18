using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServerGUI.Models
{
    public class PermissionBindingInformation
    {
        public PermissionBindingInformation(PermissionInformation permissionInformation)
        {
            RoleName = permissionInformation.RoleName;
            DeviceName = permissionInformation.DeviceName;
            CanCreate = permissionInformation.CanCreate;
            CanRead = permissionInformation.CanRead;
            CanUpdate = permissionInformation.CanUpdate;
            CanDelete = permissionInformation.CanDelete;
        }
        public string RoleName { get; set; }
        public string DeviceName { get; set; }
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public bool EditButton { get; set; } = false;
    }
}
