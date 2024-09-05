using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using VehiclesControl.Application.Authentication;
using VehiclesControl.Domain.Interfaces.EntityFramework;

namespace VehiclesControl.Infrastructure.Configurations
{
    public class SwaggerJwtInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddSwaggerGen(setup =>
            {
                var jwtSecuritySheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_Only_** your JWT Bearer token an textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }

                };
                setup.AddSecurityDefinition(jwtSecuritySheme.Reference.Id, jwtSecuritySheme);
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecuritySheme,Array.Empty<string>() }
    });
            });

            services.AddScoped<IJwtProvider, JwtProvider>();
        }
    }
}
