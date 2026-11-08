using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VehiclesControl.Application.Abstractions;

namespace VehiclesControl.Data.Context;

/// <summary>Used by "dotnet ef" only. The connection string is not used to generate migrations.</summary>
public sealed class DesignTimeContextFactory : IDesignTimeDbContextFactory<VehiclesControlContext>
{
    public VehiclesControlContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<VehiclesControlContext>()
            .UseSqlServer("Server=.;Database=VehiclesControlGallery;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        return new VehiclesControlContext(options, new SystemTenantContext());
    }
}
