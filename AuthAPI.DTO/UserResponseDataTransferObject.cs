using AuthAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthAPI.DTO
{
    public class UserResponseDataTransferObject : IResponseDataTransferObject
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }  
    }
}
