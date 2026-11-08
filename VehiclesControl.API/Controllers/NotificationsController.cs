using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehiclesControl.Application.Authorization;
using VehiclesControl.Application.Notifications;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.API.Controllers;

[Authorize(Policy = AppPolicies.GalleryMember)]
[Route("api/v1/notifications")]
public sealed class NotificationsController(INotificationService notifications) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<NotificationListResponse>>> Latest(CancellationToken ct) =>
        ToResult(await notifications.GetLatestAsync(ct));

    [HttpPost("{id:long}/read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkRead(long id, CancellationToken ct) =>
        ToResult(await notifications.MarkReadAsync(id, ct));

    [HttpPost("read-all")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAllRead(CancellationToken ct) =>
        ToResult(await notifications.MarkAllReadAsync(ct));
}
