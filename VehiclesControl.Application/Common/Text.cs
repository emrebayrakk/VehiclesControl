namespace VehiclesControl.Application.Common;

internal static class Text
{
    public static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public static string Email(string value) => value.Trim().ToLowerInvariant();

    /// <summary>"34 abc 123" → "34 ABC 123" (single spaces, upper case).</summary>
    public static string? Plate(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : string.Join(' ', value.Trim().ToUpperInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
