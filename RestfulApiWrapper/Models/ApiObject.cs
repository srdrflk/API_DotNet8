using RestfulApiWrapper.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestfulApiWrapper.Models
{
    public class ApiObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [ValidProductData(ErrorMessage = "Invalid product data")]
        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
}
