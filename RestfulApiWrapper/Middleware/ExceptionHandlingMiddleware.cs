using RestfulApiWrapper.Exceptions;
using RestfulApiWrapper.Models;
using System.Net;

namespace RestfulApiWrapper.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";

            ApiErrorResponse response;

            if (exception is ValidationException valEx)
            {
                response = ApiErrorResponse.Create(
                    context,
                    valEx.StatusCode,
                    valEx.Message,
                    errors: valEx.Errors);
            }
            else if (exception is ApiException apiEx)
            {
                response = ApiErrorResponse.Create(
                    context,
                    apiEx.StatusCode,
                    apiEx.Message,
                    type: $"https://httpstatuses.com/{apiEx.StatusCode}");
            }
            else if (exception is NotFoundException notFoundEx)
            {
                response = ApiErrorResponse.Create(
                    context,
                    StatusCodes.Status404NotFound,
                    notFoundEx.Message,
                    type: "https://httpstatuses.com/404");
            }
            else if (exception is HttpRequestException httpEx && httpEx.StatusCode == HttpStatusCode.NotFound)
            {
                response = ApiErrorResponse.Create(
                    context,
                    StatusCodes.Status404NotFound,
                    "The requested resource was not found",
                    type: "https://httpstatuses.com/404");
            }
            else
            {
                response = ApiErrorResponse.FromException(
                    exception,
                    context,
                    includeDetails: _env.IsDevelopment());
            }

            context.Response.StatusCode = response.Status;
            await context.Response.WriteAsJsonAsync(response);
        }

    }
}
