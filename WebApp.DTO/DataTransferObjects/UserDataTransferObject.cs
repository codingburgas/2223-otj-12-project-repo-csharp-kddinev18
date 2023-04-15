using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApp.ValidationAttributes;

namespace WebApp.DataTransferObjects
{
    public class UserDataTransferObject
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "UserName is required.")]
        [MinLength(8, ErrorMessage = "The UserName must be at least 8 characters")]
        [MaxLength(32, ErrorMessage = "The UserName must be under 32 characters")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "The Password must be at least 6 characters")]
        [MaxLength(32, ErrorMessage = "The Password must be under 32 characters")]
        [Password]
        public string Password { get; set; }
    }
}
