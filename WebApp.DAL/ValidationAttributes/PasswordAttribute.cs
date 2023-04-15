using System.ComponentModel.DataAnnotations;

namespace WebApp.ValidationAttributes
{
    public class PasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string password = value.ToString();
                if (password.Any(char.IsUpper) &&
                    password.Any(char.IsLower) &&
                    password.Any(c => !char.IsLetterOrDigit(c)))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult($"The {validationContext.DisplayName} field must contain at least one uppercase letter, one lowercase letter, and one special character.");
                }
            }

            return new ValidationResult($"The {validationContext.DisplayName} field must contain at least one uppercase letter, one lowercase letter, and one special character.");

        }
    }

}
