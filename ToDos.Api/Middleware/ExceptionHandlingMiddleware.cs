using System.Net;
using System.Text.Json;
using ToDos.Domain.Exceptions;

namespace ToDos.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorCode = "InternalServerError";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            if (exception is NotFoundException)
            {
                errorCode = "NotFound";
                statusCode = HttpStatusCode.NotFound;
                message = "The requested resource could not be found.";
            }
            else if (exception is ArgumentException)
            {
                errorCode = "BadRequest";
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception is ArgumentNullException)
            {
                errorCode = "BadRequest";
                statusCode = HttpStatusCode.BadRequest;
                message = "Required parameter is missing or null.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                errorCode = "Unauthorized";
                statusCode = HttpStatusCode.Unauthorized;
                message = "Access denied.";
            }
            else if (exception is TimeoutException)
            {
                errorCode = "RequestTimeout";
                statusCode = HttpStatusCode.RequestTimeout;
                message = "The request timed out.";
            }

            var response = new { 
                errorCode, 
                message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path
            };
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
