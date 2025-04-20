using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace JwtMinimalAPI.Middlewere
{
    public class ExceptionHandlingMiddleware(RequestDelegate _next, ILogger<ExceptionHandlingMiddleware> _logger) // to handle exceptions in the application globally
    {
        public async Task InvokeAsync(HttpContext context) // heart of the middleware
        {
            try
            {
                await _next(context); // if no error found go to next middleware
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ValidationException)
                {
                    _logger.LogWarning(ex, "Validation error: {Message}", ex.Message);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    _logger.LogError(ex, "Unexpected error occurred: {Message}", ex.Message);
                }
                else
                {
                    _logger.LogError("Unexpected error");
                }

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception is ArgumentException || exception is ValidationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                return CreateResponse(context, "Validation error check  input data");
            }
            else if (exception is KeyNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return CreateResponse(context, "Cant be found");
            }
            // Add more errors
            else
            {
                // Standardfel - 500 Internal Server Error
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return CreateResponse(context, "Error Error ");
            }
        }

        public static Task CreateResponse(HttpContext context, string message)
        {
            var response = new
            {
                context.Response.StatusCode,
                Message = message
            };
            
            var options = new JsonSerializerOptions 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(json);
        }
    }
    
    // Extension method for easy registration in Program.cs
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
