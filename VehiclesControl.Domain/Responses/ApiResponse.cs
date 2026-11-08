using System.Text.Json.Serialization;

namespace VehiclesControl.Domain.Responses;

/// <summary>Codes carried in <see cref="ApiResponse{T}.Code"/>; they are also the HTTP status codes the API returns.</summary>
public static class ResultCode
{
    public const int Ok = 200;
    public const int Created = 201;
    public const int BadRequest = 400;
    public const int Unauthorized = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int Conflict = 409;
}

public sealed class ApiResponse<T>
{
    [JsonConstructor]
    public ApiResponse(bool status, int code, string message, T? data)
    {
        Status = status;
        Code = code;
        Message = message;
        Data = data;
    }

    public bool Status { get; init; }
    public int Code { get; init; }
    public string Message { get; init; }
    public T? Data { get; init; }

    public static ApiResponse<T> Success(T data, string message = "Success") => new(true, ResultCode.Ok, message, data);
    public static ApiResponse<T> Created(T data, string message = "Created") => new(true, ResultCode.Created, message, data);
    public static ApiResponse<T> Fail(int code, string message) => new(false, code, message, default);
    public static ApiResponse<T> NotFound(string message = "Kayıt bulunamadı.") => Fail(ResultCode.NotFound, message);
    public static ApiResponse<T> BadRequest(string message) => Fail(ResultCode.BadRequest, message);
    public static ApiResponse<T> Conflict(string message) => Fail(ResultCode.Conflict, message);
    public static ApiResponse<T> Forbidden(string message) => Fail(ResultCode.Forbidden, message);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
