using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Requests;

public sealed class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    /// <summary>Required only when <see cref="NewPassword"/> is set.</summary>
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}

public sealed class GalleryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
}

/// <summary>Super admin: creates a gallery together with its owner account.</summary>
public sealed class CreateGalleryRequest
{
    public GalleryRequest Gallery { get; set; } = new();
    public string OwnerFirstName { get; set; } = string.Empty;
    public string OwnerLastName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string? OwnerPhone { get; set; }
    public string OwnerPassword { get; set; } = string.Empty;
}

public sealed class TeamMemberRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.GalleryStaff;
}

public sealed record ActiveRequest(bool IsActive);

public sealed class VehicleRequest
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Trim { get; set; }
    public int Year { get; set; } = DateTime.Today.Year;
    public int Kilometers { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType Transmission { get; set; }
    public BodyType BodyType { get; set; }
    public string Color { get; set; } = string.Empty;
    public int? EngineVolume { get; set; }
    public int? EnginePower { get; set; }
    public string? Plate { get; set; }
    public string? Vin { get; set; }
    public decimal? DamageRecordAmount { get; set; }
    public string? PaintedParts { get; set; }
    public string? ReplacedParts { get; set; }
    public DateTime? PurchaseDate { get; set; } = DateTime.Today;
    public decimal PurchasePrice { get; set; }
    public string? SellerName { get; set; }
    public string? SellerPhone { get; set; }
    public decimal ListPrice { get; set; }
    public string? Notes { get; set; }
}

public sealed record VehicleStatusRequest(VehicleStatus Status);

public sealed class CustomerRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Notes { get; set; }
}

public sealed class SaleRequest
{
    public long VehicleId { get; set; }
    public long CustomerId { get; set; }
    public DateTime? SaleDate { get; set; } = DateTime.Today;
    public decimal SalePrice { get; set; }
    public decimal Deposit { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

public sealed class ExpenseRequest
{
    public long? VehicleId { get; set; }
    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; } = DateTime.Today;
    public string? Description { get; set; }
}

public sealed class ReminderRequest
{
    public long VehicleId { get; set; }
    public ReminderType Type { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
}

// ----- Query strings -----

public class PagedQuery
{
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool Descending { get; set; } = true;
}

public sealed class VehicleQuery : PagedQuery
{
    public VehicleStatus? Status { get; set; }
}

public sealed class SaleQuery : PagedQuery
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public long? CustomerId { get; set; }
}

public sealed class ExpenseQuery : PagedQuery
{
    public long? VehicleId { get; set; }
    public ExpenseCategory? Category { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public sealed class ReminderQuery
{
    public ReminderScope Scope { get; set; } = ReminderScope.Upcoming;
    public long? VehicleId { get; set; }
}
