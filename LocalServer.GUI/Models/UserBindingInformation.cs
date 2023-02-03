using LocalServer.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LocalServerGUI.Models
{
    public class UserBindingInformation
    {
        public UserBindingInformation(UserInformation userInformation)
        {
            UserId = userInformation.UserId;
            UserName = userInformation.UserName;
            Email = userInformation.Email;
            Role = userInformation.Role;
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public Brush BgColor { get; set; }
        public string Initials { get; set; }

        public bool EditButton { get; set; } = false;
        public bool RemoveButton { get; set; } = false;
    }
}
