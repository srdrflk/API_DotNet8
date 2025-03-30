using Microsoft.AspNetCore.Mvc.ModelBinding;
using RestfulApiWrapper.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace RestfulApiWrapper.Models
{
    public class ApiErrorResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
        public string TraceId { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Factory methods
        public static ApiErrorResponse Create(HttpContext context, int statusCode, string title, string detail = null, Dictionary<string, string[]> errors = null, string type = null)
        {
            return new ApiErrorResponse
            {
                Type = type ?? $"https://httpstatuses.com/{statusCode}",
                Title = title,
                Status = statusCode,
                Detail = detail,
                Instance = context.Request.Path,
                TraceId = context.TraceIdentifier,
                Errors = errors
            };
        }

        public static ApiErrorResponse FromModelState(ModelStateDictionary modelState, HttpContext context, string detail = "Validation error occurred")
        {
            var errors = modelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors
                        .Select(e => e.ErrorMessage).ToArray());

            return Create(context, StatusCodes.Status400BadRequest, "Invalid request", detail, errors);
        }

        public static ApiErrorResponse FromException(Exception ex, HttpContext context, bool includeDetails = false)
        {
            return Create(context, StatusCodes.Status500InternalServerError, "An error occurred", includeDetails ? ex.ToString() : "An unexpected error occurred");
        }
    }
}