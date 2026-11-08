using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Notifications;
using VehiclesControl.Data.Context;
using VehiclesControl.Hangfire;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

// Background work runs outside any gallery; the generator bypasses tenant filters on purpose.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ITenantContext, SystemTenantContext>();
builder.Services.AddDbContext<VehiclesControlContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<VehiclesControlContext>());
builder.Services.AddScoped<ReminderNotificationGenerator>();
builder.Services.AddScoped<ReminderJob>();

// Hangfire keeps its own tables (schema "HangFire") in the application database.
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

var app = builder.Build();

app.Services.GetRequiredService<IRecurringJobManager>().AddOrUpdate<ReminderJob>(
    "reminder-notifications",
    job => job.RunAsync(),
    "0 8 * * *",
    new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

var dashboardPassword = app.Configuration["HangfireSettings:Password"];
if (string.IsNullOrWhiteSpace(dashboardPassword))
{
    app.Logger.LogWarning("HangfireSettings:Password is empty; the dashboard is not exposed.");
}
else
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization =
        [
            new HangfireCustomBasicAuthenticationFilter
            {
                User = app.Configuration["HangfireSettings:UserName"],
                Pass = dashboardPassword,
            },
        ],
    });
}

app.MapGet("/", () => Results.Redirect("/hangfire"));
app.Run();
