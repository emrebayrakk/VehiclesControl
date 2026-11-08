using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Reminders;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/reminders")]
public sealed class RemindersController(IReminderService reminders) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ReminderResponse>>>> List([FromQuery] ReminderQuery query, CancellationToken ct) =>
        ToResult(await reminders.GetAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReminderResponse>>> Create(ReminderRequest request, CancellationToken ct) =>
        ToResult(await reminders.CreateAsync(request, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ApiResponse<ReminderResponse>>> Update(long id, ReminderRequest request, CancellationToken ct) =>
        ToResult(await reminders.UpdateAsync(id, request, ct));

    [HttpPost("{id:long}/complete")]
    public async Task<ActionResult<ApiResponse<ReminderResponse>>> Complete(long id, CancellationToken ct) =>
        ToResult(await reminders.CompleteAsync(id, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(long id, CancellationToken ct) =>
        ToResult(await reminders.DeleteAsync(id, ct));
}
