namespace PopsicleFactory.Api.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("[{Timestamp}] Request received: {Method} {Path}",
            DateTimeOffset.UtcNow,
            context.Request.Method,
            context.Request.Path);
        
        return next(context);
    }
}