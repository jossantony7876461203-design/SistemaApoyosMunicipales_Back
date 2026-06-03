using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Aquí distingue el tipo de excepción y devuelve el código correcto
            context.Response.StatusCode = exception switch
            {
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,    // 401
                NotFoundException => (int)HttpStatusCode.NotFound,        // 404
                ValidationException => (int)HttpStatusCode.BadRequest,      // 400
                _ => (int)HttpStatusCode.InternalServerError // 500
            };

            var response = new
            {
                statusCode = context.Response.StatusCode,
                mensaje = exception.Message
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }
    }
}