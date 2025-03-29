using RestfulApiWrapper.Attributes;
using System.ComponentModel.DataAnnotations;

namespace RestfulApiWrapper.Models
{
    public class UpdateObjectRequest
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.]+$",
        ErrorMessage = "Name can only contain alphanumeric characters, spaces, hyphens, and periods")]
        public string Name { get; set; }

        [MinLength(1, ErrorMessage = "Data must contain at least one property")]
        [ValidProductData(ErrorMessage = "Invalid product data")]
        public Dictionary<string, object> Data { get; set; }
    }
}
