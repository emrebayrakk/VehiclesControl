using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Entities;

/// <summary>Inspection / insurance / maintenance due date for a vehicle.</summary>
public class Reminder : TenantEntity
{
    public long VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public ReminderType Type { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }

    /// <summary>Set by the background job once a notification has been created.</summary>
    public DateTime? NotifiedAt { get; set; }
}

public class Notification : TenantEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; }
}
