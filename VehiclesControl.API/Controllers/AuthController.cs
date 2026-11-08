using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VehiclesControl.Application.Users;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController(IAuthService auth) : ApiControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitPolicies.Login)]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request, CancellationToken ct) =>
        ToResult(await auth.LoginAsync(request, ct));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Me(CancellationToken ct) =>
        ToResult(await auth.GetCurrentAsync(CurrentUserId, ct));

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateProfile(UpdateProfileRequest request, CancellationToken ct) =>
        ToResult(await auth.UpdateProfileAsync(CurrentUserId, request, ct));
}
