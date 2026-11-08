using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Entities;

public class Sale : TenantEntity
{
    public long VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime SaleDate { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Deposit { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }

    public long? SoldByUserId { get; set; }
    public User? SoldBy { get; set; }
}
