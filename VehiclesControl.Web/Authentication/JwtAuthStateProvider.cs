using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Web.Authentication;

/// <summary>
/// Keeps the API's JWT and the signed-in user in protected local storage and exposes them as the
/// Blazor authentication state. Expired tokens are treated as signed out.
/// </summary>
public sealed class JwtAuthStateProvider(ProtectedLocalStorage storage, ILogger<JwtAuthStateProvider> logger)
    : AuthenticationStateProvider
{
    public const string RoleClaim = "role";
    public const string GalleryNameClaim = "gallery_name";

    private const string TokenKey = "auth:token";
    private const string UserKey = "auth:user";
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private string? _token;
    private UserResponse? _user;
    private bool _loaded;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenAsync();
        return token is null ? Anonymous : new AuthenticationState(BuildPrincipal(token));
    }

    public async Task<string?> GetTokenAsync()
    {
        await EnsureLoadedAsync();
        return _token is not null && !IsExpired(_token) ? _token : null;
    }

    public async Task<UserResponse?> GetUserAsync()
    {
        await EnsureLoadedAsync();
        return _user;
    }

    public async Task SignInAsync(LoginResponse login)
    {
        _token = login.Token;
        _user = login.User;
        _loaded = true;
        await storage.SetAsync(TokenKey, login.Token);
        await storage.SetAsync(UserKey, login.User);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task UpdateUserAsync(UserResponse user)
    {
        _user = user;
        await storage.SetAsync(UserKey, user);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync()
    {
        _token = null;
        _user = null;
        _loaded = true;
        try
        {
            await storage.DeleteAsync(TokenKey);
            await storage.DeleteAsync(UserKey);
        }
        catch (Exception ex) when (ex is InvalidOperationException or TaskCanceledException)
        {
            // The circuit is going away; nothing left to clean up on the client.
        }
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private async Task EnsureLoadedAsync()
    {
        if (_loaded)
            return;

        try
        {
            var token = await storage.GetAsync<string>(TokenKey);
            var user = await storage.GetAsync<UserResponse>(UserKey);
            _token = token.Success ? token.Value : null;
            _user = user.Success ? user.Value : null;
        }
        catch (Exception ex)
        {
            // Typically the data-protection keys changed and the stored values can't be decrypted.
            logger.LogInformation(ex, "Could not read stored session; starting signed out.");
            _token = null;
            _user = null;
        }

        _loaded = true;
    }

    private static bool IsExpired(string token)
    {
        try
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo <= DateTime.UtcNow;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    private ClaimsPrincipal BuildPrincipal(string token)
    {
        var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims
            .Where(c => c.Type is not (JwtRegisteredClaimNames.Name or JwtRegisteredClaimNames.Email))
            .ToList();

        // Prefer the stored profile so a renamed user shows the new name before the token is renewed.
        if (_user is not null)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, $"{_user.FirstName} {_user.LastName}".Trim()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, _user.Email));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt", JwtRegisteredClaimNames.Name, RoleClaim));
    }
}
