using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;

namespace VehiclesControl.Web.Theme;

/// <summary>Per-circuit dark-mode preference, remembered in local storage; defaults to the OS setting.</summary>
public sealed class ThemeState(ProtectedLocalStorage storage)
{
    private const string Key = "ui:dark";
    private bool _loaded;

    public bool IsDark { get; private set; }

    public event Action? Changed;

    public async Task EnsureLoadedAsync(MudThemeProvider provider)
    {
        if (_loaded)
            return;
        _loaded = true;

        try
        {
            var stored = await storage.GetAsync<bool>(Key);
            IsDark = stored.Success ? stored.Value : await provider.GetSystemDarkModeAsync();
        }
        catch (Exception)
        {
            IsDark = false;
        }

        Changed?.Invoke();
    }

    public async Task ToggleAsync()
    {
        IsDark = !IsDark;
        Changed?.Invoke();
        try
        {
            await storage.SetAsync(Key, IsDark);
        }
        catch (Exception)
        {
            // Preference just won't be remembered.
        }
    }
}
