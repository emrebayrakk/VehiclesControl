using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Entities;

/// <summary>A vehicle in a gallery's stock.</summary>
public class Vehicle : TenantEntity
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Trim { get; set; }
    public int Year { get; set; }
    public int Kilometers { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType Transmission { get; set; }
    public BodyType BodyType { get; set; }
    public string Color { get; set; } = string.Empty;
    public int? EngineVolume { get; set; }
    public int? EnginePower { get; set; }

    public string? Plate { get; set; }
    public string? Vin { get; set; }

    /// <summary>Total of insurance damage records (TRAMER).</summary>
    public decimal? DamageRecordAmount { get; set; }
    public string? PaintedParts { get; set; }
    public string? ReplacedParts { get; set; }

    public DateTime PurchaseDate { get; set; }
    public decimal PurchasePrice { get; set; }
    public string? SellerName { get; set; }
    public string? SellerPhone { get; set; }

    public decimal ListPrice { get; set; }
    public VehicleStatus Status { get; set; }
    public string? Notes { get; set; }

    public ICollection<VehiclePhoto> Photos { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
    public ICollection<Reminder> Reminders { get; set; } = [];
    public Sale? Sale { get; set; }

    public string Title => $"{Year} {Brand} {Model}{(string.IsNullOrWhiteSpace(Trim) ? "" : " " + Trim)}";
}

public class VehiclePhoto : TenantEntity
{
    public long VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    /// <summary>Path relative to the upload root, e.g. "12/34/abc.jpg".</summary>
    public string Path { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int SortOrder { get; set; }
}
