using System.Net;
using System.Text.Json;

namespace TATA.GestiondeTalentoMoviles.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log detallado del error
            _logger.LogError(exception, "Ocurrió un error no controlado: {Message}", exception.Message);

            context.Response.ContentType = "application/json";
            
            // Determinar el código de estado basado en el tipo de excepción
            var statusCode = exception switch
            {
                ArgumentNullException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.BadRequest // Usar 400 en lugar de 500 para evitar que el sistema se caiga
            };

            context.Response.StatusCode = (int)statusCode;

            var response = _environment.IsDevelopment()
                ? new ErrorResponse
                {
                    Message = "Ocurrió un error en la aplicación",
                    StatusCode = (int)statusCode,
                    Error = exception.Message,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException?.Message,
                    Timestamp = DateTime.UtcNow
                }
                : new ErrorResponse
                {
                    Message = "Ocurrió un error en la aplicación",
                    StatusCode = (int)statusCode,
                    Error = exception.Message,
                    Timestamp = DateTime.UtcNow
                };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Error { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
