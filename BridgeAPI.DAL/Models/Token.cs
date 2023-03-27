using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.DAL.Models
{
    public class Token
    {
        public Guid TokenId { get; set; }
        public string SecretKey { get; set; }
        public Guid GlobalServerId { get; set; }
        public Guid LocalServerId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime RenewDate { get; set; }
    }
}
