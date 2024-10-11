using Hangfire;
using HangfireBasicAuthenticationFilter;
using VehiclesControl.Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire
    (x => {
        var conn = "Data Source=DESKTOP-P87BUPQ;Initial Catalog=Hangfire;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        x.UseSqlServerStorage(conn);
        RecurringJob.AddOrUpdate<Job>("busList", (y) => y.SendBusList(), Cron.Daily);
    }
    );
builder.Services.AddHangfireServer();

var app = builder.Build();

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
