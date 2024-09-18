
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehiclesControl.Application.Boat;
using VehiclesControl.Application.Bus;
using VehiclesControl.Application.Car;
using VehiclesControl.Application.RabbitMq;
using VehiclesControl.Application.User;
using VehiclesControl.Data.Context;
using VehiclesControl.Data.Repositories.CachedRepositories;
using VehiclesControl.Data.Repositories.Dapper;
using VehiclesControl.Data.Repositories.Dapper.DapperORM;
using VehiclesControl.Data.Repositories.EntityFramework;
using VehiclesControl.Domain.Interfaces.Dapper;
using VehiclesControl.Domain.Interfaces.EntityFramework;

namespace VehiclesControl.Infrastructure.Configurations
{
    public class DIServiceInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            services.Configure<ContextOption>(configuration.GetSection("ConnectionStrings"));

            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUserService, UserService>();

            //decorator configuration 1
            //services.AddScoped<ICarRepo>(provider => {
            //    var context = provider.GetService<VehiclesControlContext>();
            //    var mapper = provider.GetService<IMapper>();
            //    var cache = provider.GetService<IMemoryCache>();

            //    return new CachedCarRepo(cache,
            //        new CarRepo(context, mapper));
            //});
            services.AddScoped<ICarDriverNotificationPublisherService, CarDriverNotificationPublisherService>();
            
            services.AddScoped<ICarRepo, CarRepo>(); //decorator configuration 2 with Scrutor
            services.Decorate<ICarRepo, CachedCarRepo>();

            services.AddScoped<ICarRepositoryDapper, CarRepositoryDapper>();

            services.AddTransient<IDapperContext, DapperContext>();
            services.AddTransient<ISqlToolsProvider, SqlToolsProvider>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<ICarService, CarService>();
            
            services.AddScoped<IBusRepo, BusRepo>();
            services.AddScoped<IBusService, BusService>();
            
            services.AddScoped<IBoatRepo, BoatRepo>();
            services.AddScoped<IBoatService, BoatService>();
        }
    }
}
