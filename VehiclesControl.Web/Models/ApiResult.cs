namespace VehiclesControl.Web.Models;

/// <summary>Outcome of an API call, already translated into something the UI can show.</summary>
public sealed record ApiResult<T>(bool IsSuccess, T? Data, string? Error, int StatusCode)
{
    public static ApiResult<T> Success(T? data, int statusCode = 200) => new(true, data, null, statusCode);
    public static ApiResult<T> Failure(string error, int statusCode) => new(false, default, error, statusCode);
}
