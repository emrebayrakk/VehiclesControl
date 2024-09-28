using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using VehiclesControl.Domain.Outs;
using VehiclesControl.Web.Authentication;

namespace VehiclesControl.Web.Components.Pages.User
{
    public partial class Usr
    {
        UserResponse model { get; set; } = new UserResponse();
        [Inject] ProtectedLocalStorage localStorage {  get; set; }
        bool success { get; set; }
        bool isShow { get; set; }
        InputType PasswordInput { get; set; } = InputType.Password;
        string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;

        protected override async Task OnInitializedAsync()
        {
            LoginUserAbout loginUser = new LoginUserAbout(localStorage);
            model = await loginUser.GetUser();
        }
        private void OnValidSubmit(EditContext context)
        {
            success = true;
            StateHasChanged();
        }
        void ButtonTestclick()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
    }

    }
}
