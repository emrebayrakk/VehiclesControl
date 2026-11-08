using VehiclesControl.Application.Notifications;

namespace VehiclesControl.Hangfire;

/// <summary>Daily: turns upcoming and overdue reminders of every gallery into in-app notifications.</summary>
public sealed class ReminderJob(ReminderNotificationGenerator generator, ILogger<ReminderJob> logger)
{
    public async Task RunAsync()
    {
        var created = await generator.GenerateAsync();
        logger.LogInformation("Reminder job finished, {Count} notifications created", created);
    }
}
