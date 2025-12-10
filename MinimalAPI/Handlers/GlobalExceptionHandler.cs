using Azure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MinimalAPI.Handlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Ghi log lỗi
            _logger.LogError(exception, "An error occurred: {Message}.", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred. Please try again later.",
                Detail = exception.Message
            };

            // Trả về phản hồi
            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Đã xử lý ngoại lệ
        }
    }
}
