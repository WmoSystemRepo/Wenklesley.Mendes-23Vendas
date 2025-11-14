namespace Api.Models;
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public static ApiResponse<T> SuccessResponse(T data, string? message = null, string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId
        };
    }
    public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, string? correlationId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId
        };
    }
}
