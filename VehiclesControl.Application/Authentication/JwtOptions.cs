using System.ComponentModel.DataAnnotations;

namespace VehiclesControl.Application.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>HMAC-SHA256 key, at least 32 characters.</summary>
    [Required, MinLength(32)]
    public string SecretKey { get; set; } = string.Empty;

    [Range(5, 60 * 24 * 30)]
    public int ExpiryMinutes { get; set; } = 60 * 12;
}
