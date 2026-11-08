using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Application.Authentication;

public sealed class JwtProvider(IOptions<JwtOptions> options, TimeProvider clock) : IJwtProvider
{
    private readonly JwtOptions _options = options.Value;

    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        var now = clock.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(_options.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, user.FullName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(AppClaims.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        if (user.GalleryId is { } galleryId)
        {
            claims.Add(new Claim(AppClaims.GalleryId, galleryId.ToString()));
            if (user.Gallery is not null)
                claims.Add(new Claim(AppClaims.GalleryName, user.Gallery.Name));
        }

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
