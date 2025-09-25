using System.Diagnostics;

namespace ToDos.Api.Middleware
{
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

        public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var endTime = DateTime.UtcNow;
                var duration = stopwatch.ElapsedMilliseconds;

                // Log performance metrics
                _logger.LogInformation(
                    "Request {Method} {Path} completed in {Duration}ms with status {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    duration,
                    context.Response.StatusCode);

                // Log slow requests (> 1 second)
                if (duration > 1000)
                {
                    _logger.LogWarning(
                        "Slow request detected: {Method} {Path} took {Duration}ms",
                        context.Request.Method,
                        context.Request.Path,
                        duration);
                }

                // Log very slow requests (> 5 seconds)
                if (duration > 5000)
                {
                    _logger.LogError(
                        "Very slow request detected: {Method} {Path} took {Duration}ms",
                        context.Request.Method,
                        context.Request.Path,
                        duration);
                }
            }
        }
    }
}
