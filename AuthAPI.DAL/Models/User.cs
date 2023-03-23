using System.ComponentModel.DataAnnotations;
using WebApp.DTO;
using WebApp.DTO.Interfaces;

namespace WebApp.DAL.Models
{
    public class User
    {
        public User() { }
        public User(IRequestDataTransferObject user) 
        {
            UserRequestDataTrasferObject dataTrasferObject = user as UserRequestDataTrasferObject;
            if (dataTrasferObject.Id.HasValue)
            {
                Id = dataTrasferObject.Id.Value;
            }
            UserName = dataTrasferObject.UserName;
            Email = dataTrasferObject.Email;
            Password = dataTrasferObject.Password;
            Salt = dataTrasferObject.Salt;
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public DateTime DateRegisterd { get; set; } = DateTime.Now;
    }
}
