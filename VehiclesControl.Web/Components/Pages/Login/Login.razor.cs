using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using MudBlazor;
using System.Text.RegularExpressions;
using VehiclesControl.Domain.Inputs;
using VehiclesControl.Domain.Outs;
using VehiclesControl.Domain.Responses;
using VehiclesControl.Web.Authentication;

namespace VehiclesControl.Web.Components.Pages.Login
{
    public partial class Login
    {
        [Inject] public ApiRequest ApiRequest { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; }

        public UserLoginRequest LoginUser { get; set; } = new();
        bool success { get; set; }
        string[] errors { get; set; } = { };
        MudTextField<string> pwField1 { get; set; }
        MudForm form { get; set; }

        private IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Password must be at least of length 8";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Password must contain at least one capital letter";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Password must contain at least one lowercase letter";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Password must contain at least one digit";
        }

        private string PasswordMatch(string arg)
        {
            if (pwField1.Value != arg)
                return "Passwords don't match";
            return null;
        }
        public async Task LoginSubmit()
        {
            var res = await ApiRequest.PostAsync<ApiResponse<LoginResponse>, UserLoginRequest>("/api/v1/Auth/Login", LoginUser);
            if (res != null && res.Data != null)
            {
                await ((CustomAuthStateProvider)AuthStateProvider).MarkUserAsAuthenticated(res.Data.Token,res.Data.User);
                NavigationManager.NavigateTo("/cars", true);
            }
        }
    }
}
