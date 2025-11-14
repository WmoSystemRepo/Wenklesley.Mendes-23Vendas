using Api.Models;
using System.Net;
using System.Text.Json;
namespace Api.Middlewares;
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado ocorreu");
            await HandleExceptionAsync(context, ex);
        }
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString();
        var code = HttpStatusCode.InternalServerError;
        ApiResponse<object> response;
        switch (exception)
        {
            case ArgumentException:
            case InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                response = ApiResponse<object>.ErrorResponse(exception.Message, correlationId: correlationId);
                break;
            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                response = ApiResponse<object>.ErrorResponse(exception.Message, correlationId: correlationId);
                break;
            default:
                response = ApiResponse<object>.ErrorResponse("Ocorreu um erro interno no servidor", correlationId: correlationId);
                break;
        }
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var result = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(result);
    }
}
