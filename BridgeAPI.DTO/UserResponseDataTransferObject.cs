using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.DTO
{
    public class UserResponseDataTransferObject : IResponseDataTransferObject
    {
        public Guid GlobalServerId { get; set; }
        public Guid LocalServerId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
