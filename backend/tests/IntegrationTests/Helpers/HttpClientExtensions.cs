using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Application.Commands;
using Api.Models;
namespace IntegrationTests.Helpers;
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public static async Task<ApiResponse<T>> PostAsync<T>(this HttpClient client, string uri, object content)
    {
        var json = JsonSerializer.Serialize(content, JsonOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(uri, httpContent);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, JsonOptions) 
                   ?? new ApiResponse<T> { Success = false, Message = responseContent };
        }
        return JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, JsonOptions) 
               ?? throw new InvalidOperationException("Resposta inválida");
    }
    public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string uri, object content)
    {
        var json = JsonSerializer.Serialize(content, JsonOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PostAsync(uri, httpContent);
    }
    public static async Task<ApiResponse<T>> GetAsync<T>(this HttpClient client, string uri)
    {
        var response = await client.GetAsync(uri);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, JsonOptions) 
                   ?? new ApiResponse<T> { Success = false, Message = responseContent };
        }
        return JsonSerializer.Deserialize<ApiResponse<T>>(responseContent, JsonOptions) 
               ?? throw new InvalidOperationException("Resposta inválida");
    }
    public static async Task<ApiResponse<List<T>>> GetListAsync<T>(this HttpClient client, string uri)
    {
        var response = await client.GetAsync(uri);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return JsonSerializer.Deserialize<ApiResponse<List<T>>>(responseContent, JsonOptions) 
                   ?? new ApiResponse<List<T>> { Success = false, Message = responseContent };
        }
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<T>>>(responseContent, JsonOptions);
        if (apiResponse == null)
            throw new InvalidOperationException("Resposta inválida");
        return new ApiResponse<List<T>>
        {
            Success = apiResponse.Success,
            Data = apiResponse.Data?.ToList() ?? new List<T>(),
            Message = apiResponse.Message,
            Errors = apiResponse.Errors,
            Timestamp = apiResponse.Timestamp,
            CorrelationId = apiResponse.CorrelationId
        };
    }
    public static async Task<HttpResponseMessage> PutAsync(this HttpClient client, string uri, object content)
    {
        var json = JsonSerializer.Serialize(content, JsonOptions);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        return await client.PutAsync(uri, httpContent);
    }
    public static async Task<HttpResponseMessage> DeleteAsync(this HttpClient client, string uri)
    {
        return await client.DeleteAsync(uri);
    }
}
