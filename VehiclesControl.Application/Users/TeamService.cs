using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Users;

/// <summary>Gallery owners manage the people who work in their gallery.</summary>
public interface ITeamService
{
    Task<ApiResponse<List<UserResponse>>> GetAllAsync(CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> CreateAsync(TeamMemberRequest request, CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> SetActiveAsync(long userId, bool isActive, CancellationToken ct = default);
}

public sealed class TeamService(IAppDbContext db, ITenantContext tenant, IPasswordHasher<User> hasher) : ITeamService
{
    private IQueryable<User> Members => db.Users.Where(u => u.GalleryId != null && u.GalleryId == tenant.GalleryId);

    public async Task<ApiResponse<List<UserResponse>>> GetAllAsync(CancellationToken ct = default) =>
        ApiResponse<List<UserResponse>>.Success(await Members
            .OrderBy(u => u.Role).ThenBy(u => u.FirstName)
            .Select(Projections.User)
            .ToListAsync(ct));

    public async Task<ApiResponse<UserResponse>> CreateAsync(TeamMemberRequest request, CancellationToken ct = default)
    {
        if (tenant.GalleryId is not { } galleryId)
            return ApiResponse<UserResponse>.Forbidden("Bu işlem için bir galeriye bağlı olmalısınız.");

        if (request.Role == UserRole.SuperAdmin)
            return ApiResponse<UserResponse>.BadRequest("Geçersiz rol.");

        var email = Text.Email(request.Email);
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            return ApiResponse<UserResponse>.Conflict("Bu e-posta adresi zaten kayıtlı.");

        var user = new User
        {
            GalleryId = galleryId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Phone = Text.Clean(request.Phone),
            Role = request.Role,
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return ApiResponse<UserResponse>.Created(await Members.Where(u => u.Id == user.Id).Select(Projections.User).FirstAsync(ct));
    }

    public async Task<ApiResponse<UserResponse>> SetActiveAsync(long userId, bool isActive, CancellationToken ct = default)
    {
        if (userId == tenant.UserId)
            return ApiResponse<UserResponse>.BadRequest("Kendi hesabınızı pasif yapamazsınız.");

        var user = await Members.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
            return ApiResponse<UserResponse>.NotFound();

        user.IsActive = isActive;
        await db.SaveChangesAsync(ct);
        return ApiResponse<UserResponse>.Success(await Members.Where(u => u.Id == userId).Select(Projections.User).FirstAsync(ct));
    }
}
