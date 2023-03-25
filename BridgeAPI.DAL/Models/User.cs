using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BridgeAPI.DAL.Models
{
    public class User
    {
        public User() { }
        public User(IRequestDataTransferObject user)
        {
            UserRequestDataTransferObject dataTrasferObject = user as UserRequestDataTransferObject;
            if (dataTrasferObject.Id.HasValue)
            {
                Id = dataTrasferObject.Id.Value;
            }
            UserName = dataTrasferObject.UserName;
            Email = dataTrasferObject.Email;
            Password = dataTrasferObject.Password;
            Salt = dataTrasferObject.Salt;
        }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime DateRegisterd { get; set; } = DateTime.Now;
    }
}
