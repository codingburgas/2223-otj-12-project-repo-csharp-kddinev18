using System.Runtime.CompilerServices;

namespace LocalServer.DTO.Models
{
    public class UserInformation
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}