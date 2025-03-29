using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RestfulApiWrapper.Attributes
{
    public sealed class ValidProductIdAttribute : ValidationAttribute
    {
        private static readonly Regex _hexIdRegex = new(
            pattern: @"^[a-f0-9]{32}$",
            options: RegexOptions.IgnoreCase | RegexOptions.Compiled,
            matchTimeout: TimeSpan.FromMilliseconds(200));

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is not string id)
            {
                return new ValidationResult("Product ID must be a string value");
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                return new ValidationResult("Product ID cannot be empty");
            }

            if (!_hexIdRegex.IsMatch(id))
            {
                return new ValidationResult("Product ID must be a 32-character hexadecimal string (example: ff808181932badb60195e41234ea6067)");
            }

            return ValidationResult.Success;
        }
    }
}

