using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Data.Context;
using VehiclesControl.Infrastructure.Storage;
using VehiclesControl.Infrastructure.Tenancy;

namespace VehiclesControl.Infrastructure.Configurations;

public sealed class DataServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetDatabaseConnectionString();

        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, HttpTenantContext>();

        services.AddDbContext<VehiclesControlContext>(options =>
        {
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());
            if (environment.IsDevelopment())
                options.EnableSensitiveDataLogging();
        });
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<VehiclesControlContext>());

        services.AddOptions<FileStorageOptions>()
            .Bind(configuration.GetSection(FileStorageOptions.SectionName))
            .PostConfigure(o =>
            {
                if (string.IsNullOrWhiteSpace(o.RootPath))
                    o.RootPath = Path.Combine(environment.ContentRootPath, "wwwroot", "uploads");
            });
        services.AddSingleton<IFileStorage, LocalFileStorage>();
    }
}
