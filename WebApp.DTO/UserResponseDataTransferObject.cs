using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DTO.Interfaces;

namespace WebApp.DTO
{
    public class UserResponseDataTransferObject : IResponseDataTransferObject
    {
        public UserResponseDataTransferObject() { }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }  
    }
}
