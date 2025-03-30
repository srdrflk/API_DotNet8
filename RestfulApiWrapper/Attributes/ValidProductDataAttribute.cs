using System.ComponentModel.DataAnnotations;

namespace RestfulApiWrapper.Attributes
{
    public class ValidProductDataAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not Dictionary<string, object> data)
            {
                return new ValidationResult("Data must be a dictionary");
            }

            // Example: Validate price if exists
            if (data.TryGetValue("price", out var priceObj))
            {
                if (!decimal.TryParse(priceObj.ToString(), out var price) || price <= 0)
                {
                    return new ValidationResult("Price must be a positive number");
                }
            }

            // Example: Validate color if exists
            if (data.TryGetValue("color", out var color))
            {
                var validColors = new[] { "Red", "Blue", "Green", "Black", "White" };
                if (!validColors.Contains(color.ToString(), StringComparer.OrdinalIgnoreCase))
                {
                    return new ValidationResult($"Color must be one of: {string.Join(", ", validColors)}");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
