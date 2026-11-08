using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Users;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> GetCurrentAsync(long userId, CancellationToken ct = default);
    Task<ApiResponse<UserResponse>> UpdateProfileAsync(long userId, UpdateProfileRequest request, CancellationToken ct = default);
}

public sealed class AuthService(IAppDbContext db, IPasswordHasher<User> hasher, IJwtProvider jwt, TimeProvider clock) : IAuthService
{
    private const string InvalidLogin = "E-posta veya şifre hatalı.";

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = Text.Email(request.Email);
        var user = await db.Users.Include(u => u.Gallery).FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null || !Verify(user, request.Password))
            return ApiResponse<LoginResponse>.Fail(ResultCode.Unauthorized, InvalidLogin);

        if (!user.IsActive)
            return ApiResponse<LoginResponse>.Forbidden("Hesabınız pasif durumda. Galeri yöneticinizle iletişime geçin.");

        if (user.Gallery is { IsActive: false })
            return ApiResponse<LoginResponse>.Forbidden("Galeriniz pasif durumda. Sistem yöneticisiyle iletişime geçin.");

        user.LastLoginAt = clock.GetUtcNow().UtcDateTime;
        await db.SaveChangesAsync(ct);

        var (token, expiresAt) = jwt.CreateToken(user);
        var profile = await db.Users.Where(u => u.Id == user.Id).Select(Projections.User).FirstAsync(ct);
        return ApiResponse<LoginResponse>.Success(new LoginResponse { User = profile, Token = token, ExpiresAt = expiresAt });
    }

    public async Task<ApiResponse<UserResponse>> GetCurrentAsync(long userId, CancellationToken ct = default) =>
        await db.Users.Where(u => u.Id == userId).Select(Projections.User).FirstOrDefaultAsync(ct) is { } user
            ? ApiResponse<UserResponse>.Success(user)
            : ApiResponse<UserResponse>.NotFound();

    public async Task<ApiResponse<UserResponse>> UpdateProfileAsync(long userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
            return ApiResponse<UserResponse>.NotFound();

        var email = Text.Email(request.Email);
        if (email != user.Email && await db.Users.AnyAsync(u => u.Email == email, ct))
            return ApiResponse<UserResponse>.Conflict("Bu e-posta adresi başka bir hesapta kullanılıyor.");

        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            if (string.IsNullOrEmpty(request.CurrentPassword) || !Verify(user, request.CurrentPassword))
                return ApiResponse<UserResponse>.BadRequest("Mevcut şifreniz hatalı.");
            user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = email;
        user.Phone = Text.Clean(request.Phone);
        await db.SaveChangesAsync(ct);

        return await GetCurrentAsync(userId, ct);
    }

    private bool Verify(User user, string password)
    {
        try
        {
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
                user.PasswordHash = hasher.HashPassword(user, password);
            return result != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            // Not a valid hash: compare in constant time and treat as failure unless identical.
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(user.PasswordHash), Encoding.UTF8.GetBytes(password));
        }
    }
}
