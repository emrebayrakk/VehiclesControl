using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Entities;

/// <summary>A cost. With a vehicle it counts towards that vehicle's cost; without one it is a general gallery expense.</summary>
public class Expense : TenantEntity
{
    public long? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}
