using System.Net;
using System.Text.Json;
using ToDos.Domain.Exceptions;

namespace ToDos.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorCode = "Error";
            var statusCode = HttpStatusCode.BadRequest;
            var message = exception.Message;

            if (exception is NotFoundException)
            {
                errorCode = "NotFound";
                statusCode = HttpStatusCode.NotFound;
                message = "Data could not be found.";
            }
            else if (exception is ArgumentException)
            {
                errorCode = "BadRequest";
                statusCode = HttpStatusCode.BadRequest;
            }

            var response = new { errorCode, message };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
