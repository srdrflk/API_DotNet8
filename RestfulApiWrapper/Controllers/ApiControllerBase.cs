using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using RestfulApiWrapper.Exceptions;

namespace RestfulApiWrapper.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected void ValidatePagination(int pageNumber, int pageSize)
        {
            var errors = new Dictionary<string, string[]>();

            if (pageNumber < 1)
                errors.Add("pageNumber", new[] { "Page number must be at least 1" });

            if (pageSize < 1 || pageSize > 100)
                errors.Add("pageSize", new[] { "Page size must be between 1 and 100" });

            if (errors.Any())
                throw new ValidationException(errors);
        }

        protected ActionResult ValidationError(ModelStateDictionary modelState)
        {
            throw new ValidationException(modelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()));
        }

        protected ActionResult NotFound(string message)
        {
            throw new NotFoundException(message);
        }
    }
}
