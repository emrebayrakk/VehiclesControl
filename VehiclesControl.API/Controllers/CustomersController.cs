using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Customers;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/customers")]
public sealed class CustomersController(ICustomerService customers) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerResponse>>>> List([FromQuery] PagedQuery query, CancellationToken ct) =>
        ToResult(await customers.GetPagedAsync(query, ct));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> Get(long id, CancellationToken ct) =>
        ToResult(await customers.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> Create(CustomerRequest request, CancellationToken ct) =>
        ToResult(await customers.CreateAsync(request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> Update(long id, CustomerRequest request, CancellationToken ct) =>
        ToResult(await customers.UpdateAsync(id, request, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id, CancellationToken ct) =>
        ToResult(await customers.DeleteAsync(id, ct));
}
