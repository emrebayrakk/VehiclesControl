using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Galleries;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

/// <summary>Super admin: manage galleries (tenants).</summary>
[Authorize(Policy = AppPolicies.SuperAdmin)]
[Route("api/v1/admin")]
public sealed class AdminController(IGalleryAdminService admin) : ApiControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<AdminSummaryResponse>>> Summary(CancellationToken ct) =>
        ToResult(await admin.GetSummaryAsync(ct));

    [HttpGet("galleries")]
    public async Task<ActionResult<ApiResponse<List<GalleryResponse>>>> Galleries([FromQuery] string? search, CancellationToken ct) =>
        ToResult(await admin.GetAllAsync(search, ct));

    [HttpPost("galleries")]
    public async Task<ActionResult<ApiResponse<GalleryResponse>>> Create(CreateGalleryRequest request, CancellationToken ct) =>
        ToResult(await admin.CreateAsync(request, ct));

    [HttpPut("galleries/{id:long}")]
    public async Task<ActionResult<ApiResponse<GalleryResponse>>> Update(long id, GalleryRequest request, CancellationToken ct) =>
        ToResult(await admin.UpdateAsync(id, request, ct));

    [HttpPatch("galleries/{id:long}/active")]
    public async Task<ActionResult<ApiResponse<GalleryResponse>>> SetActive(long id, ActiveRequest request, CancellationToken ct) =>
        ToResult(await admin.SetActiveAsync(id, request.IsActive, ct));
}
