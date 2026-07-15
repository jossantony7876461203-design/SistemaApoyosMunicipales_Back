using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaApoyosMunicipales.Domain.Exceptions;

namespace SistemaApoyosMunicipales.API.Middlewares
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Error durante la petición {Method} {Path}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    exception.Message);

                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw exception;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            var statusCode = GetStatusCode(exception);
            var message = GetMessage(exception);

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                statusCode,
                message,
                mensaje = message,
                traceId = context.TraceIdentifier
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }

        private static int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                UnauthorizedException =>
                    (int)HttpStatusCode.Unauthorized,

                NotFoundException =>
                    (int)HttpStatusCode.NotFound,

                ValidationException =>
                    (int)HttpStatusCode.BadRequest,

                BadRequestException =>
                    (int)HttpStatusCode.BadRequest,

                DbUpdateException =>
                    (int)HttpStatusCode.BadRequest,

                _ =>
                    (int)HttpStatusCode.InternalServerError
            };
        }

        private string GetMessage(Exception exception)
        {
            return exception switch
            {
                UnauthorizedException =>
                    exception.Message,

                NotFoundException =>
                    exception.Message,

                ValidationException =>
                    exception.Message,

                BadRequestException =>
                    exception.Message,

                DbUpdateException dbUpdateException =>
                    GetDatabaseMessage(dbUpdateException),

                _ when _environment.IsDevelopment() =>
                    exception.InnerException?.Message
                    ?? exception.Message,

                _ =>
                    "Ocurrió un error interno en el servidor."
            };
        }

        private string GetDatabaseMessage(
            DbUpdateException exception)
        {
            if (_environment.IsDevelopment())
            {
                return exception.InnerException?.Message
                    ?? exception.Message;
            }

            return "No se pudo guardar la información en la base de datos.";
        }
    }
}