using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace VehiclesControl.Infrastructure.Configurations;

public interface IServiceInstaller
{
    void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
}
