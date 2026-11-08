using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Sales;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/sales")]
public sealed class SalesController(ISaleService sales) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<SaleListResponse>>> List([FromQuery] SaleQuery query, CancellationToken ct) =>
        ToResult(await sales.GetPagedAsync(query, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<SaleResponse>>> Get(long id, CancellationToken ct) =>
        ToResult(await sales.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<SaleResponse>>> Create(SaleRequest request, CancellationToken ct) =>
        ToResult(await sales.CreateAsync(request, ct));

    /// <summary>Cancels the sale; the vehicle goes back to stock. Owners only.</summary>
    [Authorize(Policy = AppPolicies.GalleryOwner)]
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(long id, CancellationToken ct) =>
        ToResult(await sales.CancelAsync(id, ct));
}
