using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Bus;
using VehiclesControl.Data.Context;
using VehiclesControl.Data.MapperProfile;
using VehiclesControl.Data.Repositories.EntityFramework;
using VehiclesControl.Domain.Interfaces.EntityFramework;
using VehiclesControl.Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<VehiclesControlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddTransient<IBusRepo, BusRepo>();
builder.Services.AddTransient<IBusService, BusService>();

builder.Services.AddTransient<HangfireJobService>();

builder.Services.AddHangfire(x =>
{
    var conn = "Data Source=DESKTOP-P87BUPQ;Initial Catalog=Hangfire;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
    x.UseSqlServerStorage(conn);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var hangfireJobService = scope.ServiceProvider.GetRequiredService<HangfireJobService>();
    hangfireJobService.ScheduleJobs(); 
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = builder.Configuration.GetSection("HangfireSettings:UserName").Value,
            Pass = builder.Configuration.GetSection("HangfireSettings:Password").Value
        }
    }
});

app.UseHttpsRedirection();

app.Run();
