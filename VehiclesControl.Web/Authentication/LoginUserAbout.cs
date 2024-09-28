﻿using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Web.Authentication
{
    public class LoginUserAbout(ProtectedLocalStorage localStorage)
    {
        public async Task<UserResponse> GetUser()
        {
            var usr = (await localStorage.GetAsync<UserResponse>("user")).Value;
            if (usr != null) 
            {
                return usr;
            }
            return null;
        }
    }
}
