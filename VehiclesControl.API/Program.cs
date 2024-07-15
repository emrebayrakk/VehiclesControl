using Serilog;
using VehiclesControl.API.Middleware;
using VehiclesControl.Infrastructure.Configurations;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services
    .InstallServices(builder.Configuration,
    typeof(IServiceInstaller).Assembly);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Use(async (context, next) =>
{
    Log.Information("Handling request: {Method} {Url}", context.Request.Method, context.Request.Path);
    await next.Invoke();
    Log.Information("Finished handling request: {Method} {Url}", context.Request.Method, context.Request.Path);
});

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
