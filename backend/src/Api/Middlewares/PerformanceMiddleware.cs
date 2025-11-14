using System.Diagnostics;
using Serilog;
namespace Api.Middlewares;
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;
    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() 
                           ?? Guid.NewGuid().ToString();
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var method = context.Request.Method;
            var path = context.Request.Path.Value;
            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            var isSlowRequest = elapsedMs > 1000;
            var logLevel = isSlowRequest ? LogLevel.Warning : LogLevel.Information;
            if (isSlowRequest)
            {
                _logger.Log(logLevel, "Request lento detectado: {Method} {Path} levou {ElapsedMs}ms (CorrelationId: {CorrelationId})",
                    method, path, elapsedMs, correlationId);
            }
            else
            {
                _logger.Log(logLevel, "Request processado: {Method} {Path} - {StatusCode} em {ElapsedMs}ms (CorrelationId: {CorrelationId})",
                    method, path, statusCode, elapsedMs, correlationId);
            }
        }
    }
}
