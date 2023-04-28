using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApp.ValidationAttributes;

namespace WebApp.DataTransferObjects
{
    public class UserRegisterDataTransferObject : UserDataTransferObject
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [MinLength(6, ErrorMessage = "The Email must be at least 6 characters")]
        [Email]
        public string Email { get; set; }

        public IFormFile Image { get; set; }
    }
}
