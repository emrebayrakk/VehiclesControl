using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Galleries;
using VehiclesControl.Application.Users;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/gallery")]
public sealed class GalleryController(IGalleryService gallery, ITeamService team) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<GalleryResponse>>> Get(CancellationToken ct) =>
        ToResult(await gallery.GetMineAsync(ct));

    [Authorize(Policy = AppPolicies.GalleryOwner)]
    [HttpPut]
    public async Task<ActionResult<ApiResponse<GalleryResponse>>> Update(GalleryRequest request, CancellationToken ct) =>
        ToResult(await gallery.UpdateMineAsync(request, ct));

    [Authorize(Policy = AppPolicies.GalleryOwner)]
    [HttpGet("team")]
    public async Task<ActionResult<ApiResponse<List<UserResponse>>>> Team(CancellationToken ct) =>
        ToResult(await team.GetAllAsync(ct));

    [Authorize(Policy = AppPolicies.GalleryOwner)]
    [HttpPost("team")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> AddMember(TeamMemberRequest request, CancellationToken ct) =>
        ToResult(await team.CreateAsync(request, ct));

    [Authorize(Policy = AppPolicies.GalleryOwner)]
    [HttpPatch("team/{id:long}/active")]
    public async Task<ActionResult<ApiResponse<UserResponse>>> SetMemberActive(long id, ActiveRequest request, CancellationToken ct) =>
        ToResult(await team.SetActiveAsync(id, request.IsActive, ct));
}
