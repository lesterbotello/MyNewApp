public class PerformanceTimer
{
    private readonly RequestDelegate _next;
    private ILogger<PerformanceTimer> _logger;

    public PerformanceTimer(ILogger<PerformanceTimer> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next.Invoke(context);
        _logger.LogInformation($"Request {context.Request.Method} {context.Request.Path} executed in {(DateTime.UtcNow - start).TotalMilliseconds}ms");

    }
}

public static class PerformanceTimerExtensions
{
    public static IApplicationBuilder UsePerformanceTimer(this IApplicationBuilder app)
    {
        return app.UseMiddleware<PerformanceTimer>();
    }
}