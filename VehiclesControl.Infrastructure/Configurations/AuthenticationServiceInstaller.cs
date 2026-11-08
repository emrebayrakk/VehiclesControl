using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VehiclesControl.Application.Authentication;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Infrastructure.Configurations;

public sealed class AuthenticationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
            {
                // Keep claim names as issued ("sub", "name", "role", "gallery_id").
                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Value.Issuer,
                    ValidAudience = jwt.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.SecretKey)),
                    NameClaimType = JwtRegisteredClaimNames.Name,
                    RoleClaimType = AppClaims.Role,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AppPolicies.SuperAdmin, p => p.RequireRole(nameof(UserRole.SuperAdmin)))
            .AddPolicy(AppPolicies.GalleryMember, p => p
                .RequireClaim(AppClaims.GalleryId)
                .RequireRole(nameof(UserRole.GalleryOwner), nameof(UserRole.GalleryStaff)))
            .AddPolicy(AppPolicies.GalleryOwner, p => p
                .RequireClaim(AppClaims.GalleryId)
                .RequireRole(nameof(UserRole.GalleryOwner)));
    }
}
