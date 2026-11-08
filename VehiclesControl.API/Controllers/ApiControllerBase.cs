using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Writes the envelope with the HTTP status code that matches <see cref="ApiResponse{T}.Code"/>.</summary>
    protected ActionResult<ApiResponse<T>> ToResult<T>(ApiResponse<T> response) => StatusCode(response.Code, response);

    protected long CurrentUserId =>
        long.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : 0;
}
