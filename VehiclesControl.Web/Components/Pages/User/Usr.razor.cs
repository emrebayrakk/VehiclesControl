using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;
using VehiclesControl.Web.Authentication;
using static MudBlazor.CategoryTypes;

namespace VehiclesControl.Web.Components.Pages.User
{
    public partial class Usr
    {
        UserResponse model { get; set; } = new UserResponse();
        UserRequest UserReq { get; set; } = new UserRequest();
        [Inject] ProtectedLocalStorage localStorage {  get; set; }
        [Inject] public ApiRequest ApiRequest { get; set; }
        bool success { get; set; }
        bool isShow { get; set; }
        InputType PasswordInput { get; set; } = InputType.Password;
        string PasswordInputIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
        [Inject] public ISnackbar Snackbar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadUser();
        }
        private async Task LoadUser()
        {
            LoginUserAbout loginUser = new LoginUserAbout(localStorage);
            model = await loginUser.GetUser();
            StateHasChanged();
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
        public async Task<bool> UpdateUser()
        {
            var mappedUser = new UserRequest
            {
                Email = model.Email,
                FirstName = model.FirstName,
                Id = model.Id,
                LastName = model.LastName,
                Password = model.Password,
            };
            var res = await ApiRequest.PutAsync<ApiResponse<Domain.Entities.User>, UserRequest>("/VehiclesControl/api/v1/Users/Update", mappedUser);
            if (res != null && res.Data != null)
            {
                LoginUserAbout loginUser = new LoginUserAbout(localStorage);
                await loginUser.DeleteUserStorage();
                var mappedUserRes = new UserResponse
                {
                    Email = res.Data.Email,
                    FirstName = res.Data.FirstName,
                    Id = res.Data.Id,
                    LastName = res.Data.LastName,
                    Password = res.Data.Password,
                };
                await loginUser.SetUserStorage(mappedUserRes);
                await LoadUser();
                Snackbar.Add($"Edited User Id: {model.Id}", Severity.Success, c => c.SnackbarVariant = Variant.Text);
                return true;
            }
            return false;
        }

    }
}
