using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Notifications;

public interface INotificationService
{
    Task<ApiResponse<NotificationListResponse>> GetLatestAsync(CancellationToken ct = default);
    Task<ApiResponse<bool>> MarkReadAsync(long id, CancellationToken ct = default);
    Task<ApiResponse<bool>> MarkAllReadAsync(CancellationToken ct = default);
}

public sealed class NotificationService(IAppDbContext db) : INotificationService
{
    public async Task<ApiResponse<NotificationListResponse>> GetLatestAsync(CancellationToken ct = default)
    {
        var unread = await db.Notifications.CountAsync(n => !n.IsRead, ct);
        var items = await db.Notifications
            .OrderByDescending(n => n.Id)
            .Take(20)
            .Select(n => new NotificationResponse(n.Id, n.Title, n.Message, n.Link, n.IsRead, n.CreatedDate))
            .ToListAsync(ct);
        return ApiResponse<NotificationListResponse>.Success(new NotificationListResponse(unread, items));
    }

    public async Task<ApiResponse<bool>> MarkReadAsync(long id, CancellationToken ct = default)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);
        if (notification is null)
            return ApiResponse<bool>.NotFound();

        notification.IsRead = true;
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> MarkAllReadAsync(CancellationToken ct = default)
    {
        var unread = await db.Notifications.Where(n => !n.IsRead).ToListAsync(ct);
        unread.ForEach(n => n.IsRead = true);
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.Success(true);
    }
}

/// <summary>
/// Run by the Hangfire job: turns reminders that fall due within <see cref="DaysAhead"/> days (or are
/// overdue) into in-app notifications for their gallery, once per reminder.
/// </summary>
public sealed class ReminderNotificationGenerator(IAppDbContext db, TimeProvider clock, ILogger<ReminderNotificationGenerator> logger)
{
    public const int DaysAhead = 14;

    public async Task<int> GenerateAsync(CancellationToken ct = default)
    {
        var today = clock.GetLocalNow().Date;
        var limit = today.AddDays(DaysAhead);

        var due = await db.Reminders.IgnoreQueryFilters()
            .Include(r => r.Vehicle)
            .Where(r => !r.IsCompleted && r.NotifiedAt == null && r.DueDate <= limit)
            .ToListAsync(ct);

        var now = clock.GetUtcNow().UtcDateTime;
        foreach (var reminder in due)
        {
            var days = (reminder.DueDate - today).Days;
            var when = days switch
            {
                < 0 => $"{-days} gün gecikti",
                0 => "bugün",
                1 => "yarın",
                _ => $"{days} gün sonra",
            };

            db.Notifications.Add(new Notification
            {
                GalleryId = reminder.GalleryId,
                Title = $"{Label(reminder.Type)} yaklaşıyor",
                Message = $"{reminder.Vehicle.Year} {reminder.Vehicle.Brand} {reminder.Vehicle.Model}{(reminder.Vehicle.Plate is null ? "" : $" ({reminder.Vehicle.Plate})")} — {when}.",
                Link = $"/vehicles/{reminder.VehicleId}",
            });
            reminder.NotifiedAt = now;
        }

        await db.SaveChangesAsync(ct);
        if (due.Count > 0)
            logger.LogInformation("Created {Count} reminder notifications", due.Count);
        return due.Count;
    }

    public static string Label(ReminderType type) => type switch
    {
        ReminderType.Inspection => "Muayene",
        ReminderType.TrafficInsurance => "Trafik sigortası",
        ReminderType.Casco => "Kasko",
        ReminderType.Maintenance => "Bakım",
        _ => "Hatırlatıcı",
    };
}
