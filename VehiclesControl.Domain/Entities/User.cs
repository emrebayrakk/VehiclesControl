using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Entities;

public class User : Entity
{
    /// <summary>Null only for super admins.</summary>
    public long? GalleryId { get; set; }
    public Gallery? Gallery { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    /// <summary>PBKDF2 hash from PasswordHasher.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}
