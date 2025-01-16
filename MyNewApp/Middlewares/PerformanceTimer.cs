public class PerformanceTimer
{
    private RequestDelegate _next;
    private ILogger<PerformanceTimer> _logger;

    public PerformanceTimer(RequestDelegate next, ILogger<PerformanceTimer> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next(context);
        var end = DateTime.UtcNow;
        _logger.LogInformation($"Request executed in {(end - start).TotalMilliseconds}ms");
    }
}

public static class PerformanceTimerExtensions
{
    public static IApplicationBuilder UsePerformanceTimer(this IApplicationBuilder app)
    {
        return app.UseMiddleware<PerformanceTimer>();
    }
}