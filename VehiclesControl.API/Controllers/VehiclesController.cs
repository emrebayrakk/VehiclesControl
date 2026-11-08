using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Vehicles;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/vehicles")]
public sealed class VehiclesController(IVehicleService vehicles) : ApiControllerBase
{
    private const long MaxUploadBytes = VehicleService.MaxPhotoBytes * 10 + 1024 * 1024;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VehicleListItem>>>> List([FromQuery] VehicleQuery query, CancellationToken ct) =>
        ToResult(await vehicles.GetPagedAsync(query, ct));

    [HttpGet("options")]
    public async Task<ActionResult<ApiResponse<List<VehicleOption>>>> Options([FromQuery] string? search, [FromQuery] bool availableOnly, CancellationToken ct) =>
        ToResult(await vehicles.GetOptionsAsync(search, availableOnly, ct));

    [HttpGet("brands")]
    public async Task<ActionResult<ApiResponse<List<string>>>> Brands(CancellationToken ct) =>
        ToResult(await vehicles.GetBrandsAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<VehicleDetail>>> Get(long id, CancellationToken ct) =>
        ToResult(await vehicles.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<VehicleDetail>>> Create(VehicleRequest request, CancellationToken ct) =>
        ToResult(await vehicles.CreateAsync(request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<VehicleDetail>>> Update(long id, VehicleRequest request, CancellationToken ct) =>
        ToResult(await vehicles.UpdateAsync(id, request, ct));

    [Authorize(Policy = AppPolicies.GalleryOwner)]
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id, CancellationToken ct) =>
        ToResult(await vehicles.DeleteAsync(id, ct));

    [HttpPatch("{id:long}/status")]
    public async Task<ActionResult<ApiResponse<VehicleDetail>>> SetStatus(long id, VehicleStatusRequest request, CancellationToken ct) =>
        ToResult(await vehicles.SetStatusAsync(id, request.Status, ct));

    [HttpPost("{id:long}/photos")]
    [RequestSizeLimit(MaxUploadBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadBytes)]
    public async Task<ActionResult<ApiResponse<List<PhotoResponse>>>> UploadPhotos(long id, [FromForm] IFormFileCollection files, CancellationToken ct)
    {
        var uploads = files.Select(f => new UploadedFile(f.OpenReadStream(), f.FileName, f.Length)).ToList();
        try
        {
            return ToResult(await vehicles.AddPhotosAsync(id, uploads, ct));
        }
        finally
        {
            foreach (var upload in uploads)
                await upload.Content.DisposeAsync();
        }
    }

    [HttpDelete("{id:long}/photos/{photoId:long}")]
    public async Task<ActionResult<ApiResponse<List<PhotoResponse>>>> DeletePhoto(long id, long photoId, CancellationToken ct) =>
        ToResult(await vehicles.DeletePhotoAsync(id, photoId, ct));

    [HttpPut("{id:long}/photos/{photoId:long}/cover")]
    public async Task<ActionResult<ApiResponse<List<PhotoResponse>>>> SetCover(long id, long photoId, CancellationToken ct) =>
        ToResult(await vehicles.SetCoverAsync(id, photoId, ct));
}
