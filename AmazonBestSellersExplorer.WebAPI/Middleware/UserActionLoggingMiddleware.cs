using System.Diagnostics;
using System.Security.Claims;

namespace AmazonBestSellersExplorer.WebAPI.Middleware
{
    public class UserActionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserActionLoggingMiddleware> _logger;

        public UserActionLoggingMiddleware(RequestDelegate next, ILogger<UserActionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            
            await _next(context);
            
            sw.Stop();

            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? context.User?.FindFirst("sub")?.Value;

            if (userId == null)
                return;

            var userName = context.User?.FindFirst(ClaimTypes.Name)?.Value 
                           ?? context.User?.FindFirst("name")?.Value;

            var method = context.Request.Method;
            var path = context.Request.Path;
            var statusCode = context.Response.StatusCode;
            var elapsedMs = sw.ElapsedMilliseconds;

            _logger.LogInformation("Wykonano: {Method} {Path} | User: {UserName} (Id: {UserId}) | Status: {StatusCode} | Czas: {ElapsedMs}ms", 
                method, path, userName, userId, statusCode, elapsedMs);
        }
    }
}
