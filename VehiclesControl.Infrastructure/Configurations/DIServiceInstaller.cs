
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehiclesControl.Application.Boat;
using VehiclesControl.Application.Bus;
using VehiclesControl.Application.Car;
using VehiclesControl.Application.User;
using VehiclesControl.Data.Repositories;
using VehiclesControl.Domain.Interfaces;

namespace VehiclesControl.Infrastructure.Configurations
{
    public class DIServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserService>();
            
            services.AddScoped<ICarRepo, CarRepo>();
            services.AddScoped<ICarService, CarService>();
            
            services.AddScoped<IBusRepo, BusRepo>();
            services.AddScoped<IBusService, BusService>();
            
            services.AddScoped<IBoatRepo, BoatRepo>();
            services.AddScoped<IBoatService, BoatService>();
        }
    }
}
