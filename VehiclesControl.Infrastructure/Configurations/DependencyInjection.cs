using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VehiclesControl.Infrastructure.Configurations;

public static class DependencyInjection
{
    /// <summary>Finds every <see cref="IServiceInstaller"/> in the given assemblies and runs it.</summary>
    public static IServiceCollection InstallServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        params Assembly[] assemblies)
    {
        var installers = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(t => typeof(IServiceInstaller).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var installer in installers)
            installer.Install(services, configuration, environment);

        return services;
    }

    /// <summary>
    /// Reads ConnectionStrings:DefaultConnection. A "{0}" placeholder is filled from the
    /// MSSQL_SA_PASSWORD environment variable (used by the Docker setup).
    /// </summary>
    public static string GetDatabaseConnectionString(this IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

        return connectionString.Contains("{0}")
            ? string.Format(connectionString, Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD"))
            : connectionString;
    }
}
