using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    [Serializable]
    public class CurrentUserModel
    {
        public int Id { get; set; }
        public string LastSeenDevice { get; set; } = string.Empty;
    }
}
