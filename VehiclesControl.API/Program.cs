using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;
using Serilog;
using VehiclesControl.API;
using VehiclesControl.API.ErrorHandling;
using VehiclesControl.API.Filters;
using VehiclesControl.API.OpenApi;
using VehiclesControl.API.Startup;
using VehiclesControl.Data.Context;
using VehiclesControl.Infrastructure.Configurations;
using VehiclesControl.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logger) => logger
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Data, application services, auth and storage register themselves through IServiceInstaller.
builder.Services.InstallServices(builder.Configuration, builder.Environment, typeof(IServiceInstaller).Assembly);

builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    if (allowedOrigins.Length == 0)
        policy.AllowAnyOrigin();
    else
        policy.WithOrigins(allowedOrigins);

    policy.AllowAnyHeader().AllowAnyMethod();
}));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(RateLimitPolicies.Login, http => RateLimitPartition.GetFixedWindowLimiter(
        http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 10, Window = TimeSpan.FromMinutes(1) }));
});

builder.Services.AddHealthChecks().AddDbContextCheck<VehiclesControlContext>("database");

builder.Services.AddOpenTelemetry().WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel")
    .AddView("http.server.request.duration", new ExplicitBucketHistogramConfiguration
    {
        Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5],
    })
    .AddPrometheusExporter());

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTitle("VehiclesControl Galeri API"));
}

app.UseHttpsRedirection();

// Vehicle photos. File names are random GUIDs; listing the folder is not possible.
var storage = app.Services.GetRequiredService<IOptions<FileStorageOptions>>().Value;
Directory.CreateDirectory(storage.RootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(storage.RootPath),
    RequestPath = storage.RequestPath,
    OnPrepareResponse = ctx => ctx.Context.Response.Headers.CacheControl = "public,max-age=604800",
});

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint();

await app.InitializeDatabaseAsync();
await app.RunAsync();

/// <summary>Exposed for WebApplicationFactory-based tests.</summary>
public partial class Program;
