
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehiclesControl.Data.Context;
using VehiclesControl.Data.MapperProfile;

namespace VehiclesControl.Infrastructure.Configurations
{
    public class DataServiceInstaller : IServiceInstaller
    {
        private const string SectionName = "GetTheNewsContext";
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VehiclesControlContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString(SectionName)));

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
