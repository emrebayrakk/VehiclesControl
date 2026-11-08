using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using VehiclesControl.Application.Notifications;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Tests.Support;

namespace VehiclesControl.Tests.Reminders;

public sealed class ReminderNotificationTests
{
    [Fact]
    public async Task Due_reminders_of_every_gallery_become_notifications_exactly_once()
    {
        var db = new TestDb();
        await using (var system = db.System())
        {
            system.Galleries.AddRange(new Gallery { Id = 1, Name = "A" }, new Gallery { Id = 2, Name = "B" });
            var va = TestDb.Vehicle(1);
            var vb = TestDb.Vehicle(2);
            system.AddRange(va, vb);
            await system.SaveChangesAsync();
            system.Reminders.AddRange(
                new Reminder { GalleryId = 1, VehicleId = va.Id, Type = ReminderType.Inspection, DueDate = DateTime.Today.AddDays(3) },
                new Reminder { GalleryId = 2, VehicleId = vb.Id, Type = ReminderType.Casco, DueDate = DateTime.Today.AddDays(-1) },
                new Reminder { GalleryId = 1, VehicleId = va.Id, Type = ReminderType.Maintenance, DueDate = DateTime.Today.AddDays(60) },
                new Reminder { GalleryId = 1, VehicleId = va.Id, Type = ReminderType.Other, DueDate = DateTime.Today, IsCompleted = true });
            await system.SaveChangesAsync();
        }

        await using (var system = db.System())
        {
            var generator = new ReminderNotificationGenerator(system, TimeProvider.System, NullLogger<ReminderNotificationGenerator>.Instance);
            (await generator.GenerateAsync()).Should().Be(2);
            (await generator.GenerateAsync()).Should().Be(0, "a reminder is only notified once");
        }

        await using var a = db.As(1);
        await using var b = db.As(2);
        (await a.Notifications.SingleAsync()).Title.Should().Contain("Muayene");
        (await b.Notifications.SingleAsync()).Message.Should().Contain("gecikti");
    }
}
