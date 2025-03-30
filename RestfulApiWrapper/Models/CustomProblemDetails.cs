using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace RestfulApiWrapper.Models
{
    public class CustomProblemDetails : ProblemDetails
    {
        public CustomProblemDetails(HttpContext httpContext, ModelStateDictionary modelState)
        {
            Title = "Validation Error";
            Detail = "One or more validation errors occurred";
            Status = StatusCodes.Status400BadRequest;
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            Instance = httpContext.Request.Path;

            Errors = modelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors
                        .Select(e => e.ErrorMessage).ToArray());
        }

        public Dictionary<string, string[]> Errors { get; set; }
    }
}
