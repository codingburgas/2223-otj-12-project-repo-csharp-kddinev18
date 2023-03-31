using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebApp.ValidationAttributes
{
    public class EmailAttribute : ValidationAttribute
    {
        private readonly Regex _regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string email = value.ToString();
                if (_regex.IsMatch(email))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"The {validationContext.DisplayName} field must be a valid email address.");
                }
            }

            return new ValidationResult($"The {validationContext.DisplayName} field must be a valid email address.");
        }
    }
}
