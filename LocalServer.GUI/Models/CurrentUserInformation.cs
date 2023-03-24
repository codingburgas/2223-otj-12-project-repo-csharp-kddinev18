using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalServer.GUI.Models
{
    public static class CurrentUserInformation
    {
        public static Guid UserId { get; set; }
        public static bool IsAdmin { get; set; }
    }
}
