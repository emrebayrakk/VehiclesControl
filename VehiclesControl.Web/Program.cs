using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using VehiclesControl.Web.Authentication;
using VehiclesControl.Web.Components;
using VehiclesControl.Web.Services;
using VehiclesControl.Web.Theme;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => options.DetailedErrors = builder.Environment.IsDevelopment());

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 4000;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

// Auth: the JWT issued by the API is kept in protected local storage.
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
builder.Services.AddScoped<ThemeState>();

var apiBaseUrl = new Uri(builder.Configuration["Api:BaseUrl"] ?? throw new InvalidOperationException("Api:BaseUrl is not configured."));

void ConfigureApiClient(IHttpClientBuilder client)
{
    if (builder.Environment.IsDevelopment())
    {
        // Accept the ASP.NET Core dev certificate of a local API even when this machine does not trust it.
        client.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (request, _, _, errors) =>
                errors == System.Net.Security.SslPolicyErrors.None || request.RequestUri?.IsLoopback == true,
        });
    }
}

ConfigureApiClient(builder.Services.AddHttpClient<ApiClient>(client => client.BaseAddress = apiBaseUrl));
ConfigureApiClient(builder.Services.AddHttpClient("media", client => client.BaseAddress = apiBaseUrl));
builder.Services.AddScoped<GalleryApi>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

// Vehicle photos are stored by the API; proxy them so the browser only talks to this site.
var mediaPath = new Regex(@"^[0-9]+/[0-9]+/[a-f0-9]{32}\.(jpg|jpeg|png|webp)$", RegexOptions.Compiled);
app.MapGet("/media/{**path}", async (string path, IHttpClientFactory factory, HttpContext http) =>
{
    if (!mediaPath.IsMatch(path))
        return Results.NotFound();

    var response = await factory.CreateClient("media").GetAsync($"uploads/{path}", HttpCompletionOption.ResponseHeadersRead, http.RequestAborted);
    if (!response.IsSuccessStatusCode)
        return Results.NotFound();

    http.Response.Headers.CacheControl = "public,max-age=604800";
    http.Response.RegisterForDispose(response);
    return Results.Stream(await response.Content.ReadAsStreamAsync(http.RequestAborted),
        response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
