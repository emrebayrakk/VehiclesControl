using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Dashboard;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/dashboard")]
public sealed class DashboardController(IDashboardService dashboard) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<DashboardResponse>>> Get(CancellationToken ct) =>
        ToResult(await dashboard.GetAsync(ct));
}
