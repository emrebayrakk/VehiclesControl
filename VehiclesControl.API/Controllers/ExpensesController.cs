using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Expenses;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/expenses")]
public sealed class ExpensesController(IExpenseService expenses) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<ExpenseListResponse>>> List([FromQuery] ExpenseQuery query, CancellationToken ct) =>
        ToResult(await expenses.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ExpenseResponse>>> Create(ExpenseRequest request, CancellationToken ct) =>
        ToResult(await expenses.CreateAsync(request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<ExpenseResponse>>> Update(long id, ExpenseRequest request, CancellationToken ct) =>
        ToResult(await expenses.UpdateAsync(id, request, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id, CancellationToken ct) =>
        ToResult(await expenses.DeleteAsync(id, ct));
}
