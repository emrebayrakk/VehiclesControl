using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using VehiclesControl.Domain.Responses;
using VehiclesControl.Web.Authentication;
using VehiclesControl.Web.Models;

namespace VehiclesControl.Web.Services;

/// <summary>
/// Typed HttpClient for VehiclesControl.API. Attaches the bearer token, understands both the
/// ApiResponse envelope and ProblemDetails, and signs the user out when the token is rejected.
/// </summary>
public sealed class ApiClient(HttpClient http, JwtAuthStateProvider auth, NavigationManager navigation, ILogger<ApiClient> logger)
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    public Task<ApiResult<T>> GetAsync<T>(string url, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Get, url, null, ct);
    public Task<ApiResult<T>> PostAsync<T>(string url, object? body = null, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Post, url, Content(body), ct);
    public Task<ApiResult<T>> PutAsync<T>(string url, object? body = null, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Put, url, Content(body), ct);
    public Task<ApiResult<T>> PatchAsync<T>(string url, object body, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Patch, url, Content(body), ct);
    public Task<ApiResult<T>> DeleteAsync<T>(string url, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Delete, url, null, ct);
    public Task<ApiResult<T>> PostFormAsync<T>(string url, MultipartFormDataContent form, CancellationToken ct = default) => SendAsync<T>(HttpMethod.Post, url, form, ct);

    private static HttpContent? Content(object? body) => body is null ? null : JsonContent.Create(body, body.GetType(), options: Json);

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string url, HttpContent? content, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, url) { Content = content };

        var token = await auth.GetTokenAsync();
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request, ct);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "API unreachable: {Method} {Url}", method, url);
            return ApiResult<T>.Failure("Sunucuya ulaşılamadı. API'nin çalıştığından emin olun.", 0);
        }

        using (response)
        {
            var status = (int)response.StatusCode;

            if (response.StatusCode == HttpStatusCode.Unauthorized && token is not null)
            {
                await auth.SignOutAsync();
                navigation.NavigateTo("/login", replace: true);
                return ApiResult<T>.Failure("Oturumunuzun süresi doldu. Lütfen tekrar giriş yapın.", status);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden && response.Content.Headers.ContentLength is 0 or null)
                return ApiResult<T>.Failure("Bu işlem için yetkiniz yok.", status);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                return ApiResult<T>.Failure("Çok fazla deneme yaptınız. Lütfen bir dakika sonra tekrar deneyin.", status);

            if (response.StatusCode == HttpStatusCode.RequestEntityTooLarge)
                return ApiResult<T>.Failure("Yüklenen dosyalar çok büyük.", status);

            var body = await response.Content.ReadAsStringAsync(ct);
            return Parse<T>(body, response.IsSuccessStatusCode, status);
        }
    }

    private static ApiResult<T> Parse<T>(string content, bool success, int status)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return success
                ? ApiResult<T>.Success(default, status)
                : ApiResult<T>.Failure(status == 403 ? "Bu işlem için yetkiniz yok." : $"İstek başarısız oldu ({status}).", status);
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            // ApiResponse<T> envelope: { status: bool, code, message, data }
            if (root.ValueKind == JsonValueKind.Object &&
                root.TryGetProperty("status", out var flag) && flag.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                var envelope = root.Deserialize<ApiResponse<T>>(Json)!;
                return envelope.Status
                    ? ApiResult<T>.Success(envelope.Data, status)
                    : ApiResult<T>.Failure(envelope.Message, status);
            }

            // ProblemDetails / ValidationProblemDetails
            if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("title", out var title))
            {
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    var messages = errors.EnumerateObject()
                        .SelectMany(e => e.Value.EnumerateArray().Select(v => v.GetString()))
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .Distinct();
                    return ApiResult<T>.Failure(string.Join(" ", messages), status);
                }

                return ApiResult<T>.Failure(status == 403 ? "Bu işlem için yetkiniz yok." : title.GetString() ?? $"İstek başarısız oldu ({status}).", status);
            }
        }
        catch (JsonException)
        {
        }

        return success
            ? ApiResult<T>.Failure("Sunucudan beklenmeyen bir yanıt geldi.", status)
            : ApiResult<T>.Failure($"İstek başarısız oldu ({status}).", status);
    }
}
