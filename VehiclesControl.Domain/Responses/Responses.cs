using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Domain.Responses;

public sealed class UserResponse
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public long? GalleryId { get; set; }
    public string? GalleryName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public sealed class LoginResponse
{
    public required UserResponse User { get; init; }
    public required string Token { get; init; }
    public DateTime ExpiresAt { get; init; }
}

public sealed class GalleryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public int UserCount { get; set; }
    public int VehicleCount { get; set; }
    public int SoldCount { get; set; }
    public string? OwnerName { get; set; }
    public string? OwnerEmail { get; set; }
}

public sealed record AdminSummaryResponse(int GalleryCount, int ActiveGalleryCount, int UserCount, int VehicleCount, int SaleCount);

public sealed class VehicleListItem
{
    public long Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string? Trim { get; set; }
    public int Year { get; set; }
    public int Kilometers { get; set; }
    public FuelType FuelType { get; set; }
    public TransmissionType Transmission { get; set; }
    public string Color { get; set; } = string.Empty;
    public string? Plate { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal ListPrice { get; set; }
    public VehicleStatus Status { get; set; }
    public string? CoverPhotoUrl { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime CreatedDate { get; set; }

    public decimal TotalCost => PurchasePrice + TotalExpenses;
}

public sealed record PhotoResponse(long Id, string Url, bool IsCover);

public sealed class SaleSummary
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Deposit { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? SoldByName { get; set; }
    public string? Notes { get; set; }
}

public sealed class VehicleDetail
{
    public long Id { get; set; }
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
    public DateTime CreatedDate { get; set; }

    public List<PhotoResponse> Photos { get; set; } = [];
    public List<ExpenseResponse> Expenses { get; set; } = [];
    public List<ReminderResponse> Reminders { get; set; } = [];
    public SaleSummary? Sale { get; set; }

    public decimal TotalExpenses => Expenses.Sum(e => e.Amount);
    public decimal TotalCost => PurchasePrice + TotalExpenses;
    public string Title => $"{Year} {Brand} {Model}{(string.IsNullOrWhiteSpace(Trim) ? "" : " " + Trim)}";
}

public sealed record VehicleOption(long Id, string Title, string? Plate, VehicleStatus Status, decimal ListPrice);

public sealed class CustomerResponse
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? City { get; set; }
    public string? Notes { get; set; }
    public int PurchaseCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime CreatedDate { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}

public sealed class SaleResponse
{
    public long Id { get; set; }
    public long VehicleId { get; set; }
    public string VehicleTitle { get; set; } = string.Empty;
    public string? Plate { get; set; }
    public long CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Deposit { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal TotalCost { get; set; }
    public string? SoldByName { get; set; }
    public string? Notes { get; set; }

    public decimal Profit => SalePrice - TotalCost;
}

public sealed record SaleTotals(int Count, decimal Revenue, decimal Profit);

public sealed record SaleListResponse(PagedResult<SaleResponse> Page, SaleTotals Totals);

public sealed class ExpenseResponse
{
    public long Id { get; set; }
    public long? VehicleId { get; set; }
    public string? VehicleTitle { get; set; }
    public ExpenseCategory Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
}

public sealed record ExpenseListResponse(PagedResult<ExpenseResponse> Page, decimal Total);

public sealed class ReminderResponse
{
    public long Id { get; set; }
    public long VehicleId { get; set; }
    public string VehicleTitle { get; set; } = string.Empty;
    public string? Plate { get; set; }
    public ReminderType Type { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }
}

public sealed record NotificationResponse(long Id, string Title, string Message, string? Link, bool IsRead, DateTime CreatedDate);

public sealed record NotificationListResponse(int UnreadCount, IReadOnlyList<NotificationResponse> Items);

public sealed record MonthlySalesPoint(int Year, int Month, int Count, decimal Revenue, decimal Profit);

public sealed class DashboardResponse
{
    public int InStockCount { get; set; }
    public int ReservedCount { get; set; }
    public decimal StockListValue { get; set; }
    public decimal StockCost { get; set; }
    public int SalesThisMonth { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal ProfitThisMonth { get; set; }
    public decimal ExpensesThisMonth { get; set; }
    public double AverageDaysInStock { get; set; }
    public int CustomerCount { get; set; }
    public List<MonthlySalesPoint> MonthlySales { get; set; } = [];
    public List<ReminderResponse> UpcomingReminders { get; set; } = [];
    public List<SaleResponse> RecentSales { get; set; } = [];
}
