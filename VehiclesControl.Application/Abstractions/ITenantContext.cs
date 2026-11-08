using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Application.Abstractions;

/// <summary>Who is making the current request. Populated from the JWT in the API.</summary>
public interface ITenantContext
{
    long? GalleryId { get; }
    long? UserId { get; }
    UserRole? Role { get; }
}

/// <summary>Used by background jobs and design-time tooling: no gallery, no user.</summary>
public sealed class SystemTenantContext : ITenantContext
{
    public long? GalleryId => null;
    public long? UserId => null;
    public UserRole? Role => null;
}
