public static class LoggingEndpoints
{
    public static void Map(WebApplication app, ILogger logger)
    {
        app.MapGet("/log", () => {
            logger.LogTrace("This is a trace log");
            logger.LogDebug("This is a debug log");
            logger.LogInformation("This is an information log");
            logger.LogWarning("This is a warning log");
            logger.LogError("This is an error log");
            logger.LogError(new ApplicationException("This is an exception log"), "This is an exception log");
            logger.LogCritical("This is a critical log");

            return Results.Ok();
        });
    }
}