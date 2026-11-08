using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Infrastructure.Tenancy;

/// <summary>Reads the gallery, user and role from the validated JWT of the current request.</summary>
public sealed class HttpTenantContext(IHttpContextAccessor accessor) : ITenantContext
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public long? GalleryId => long.TryParse(User?.FindFirstValue(AppClaims.GalleryId), out var id) ? id : null;

    public long? UserId => long.TryParse(User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;

    public UserRole? Role => Enum.TryParse<UserRole>(User?.FindFirstValue(AppClaims.Role), out var role) ? role : null;
}
