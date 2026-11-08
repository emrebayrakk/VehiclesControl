using MudBlazor;
using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Web.Models;

/// <summary>Turkish labels, colours and icons for the domain enums.</summary>
public static class Labels
{
    public static string Text(VehicleStatus value) => value switch
    {
        VehicleStatus.InStock => "Stokta",
        VehicleStatus.Reserved => "Rezerve",
        VehicleStatus.Sold => "Satıldı",
        _ => value.ToString(),
    };

    public static Color Color(VehicleStatus value) => value switch
    {
        VehicleStatus.InStock => MudBlazor.Color.Success,
        VehicleStatus.Reserved => MudBlazor.Color.Warning,
        _ => MudBlazor.Color.Default,
    };

    public static string Text(FuelType value) => value switch
    {
        FuelType.Gasoline => "Benzin",
        FuelType.Diesel => "Dizel",
        FuelType.Lpg => "Benzin & LPG",
        FuelType.Hybrid => "Hibrit",
        FuelType.Electric => "Elektrik",
        _ => value.ToString(),
    };

    public static string Text(TransmissionType value) => value switch
    {
        TransmissionType.Manual => "Manuel",
        TransmissionType.Automatic => "Otomatik",
        TransmissionType.SemiAutomatic => "Yarı otomatik",
        _ => value.ToString(),
    };

    public static string Text(BodyType value) => value switch
    {
        BodyType.Sedan => "Sedan",
        BodyType.Hatchback => "Hatchback",
        BodyType.StationWagon => "Station wagon",
        BodyType.Suv => "SUV",
        BodyType.Coupe => "Coupe",
        BodyType.Cabrio => "Cabrio",
        BodyType.Mpv => "MPV",
        BodyType.Pickup => "Pick-up",
        BodyType.Van => "Panelvan",
        _ => "Diğer",
    };

    public static string Text(PaymentMethod value) => value switch
    {
        PaymentMethod.Cash => "Nakit",
        PaymentMethod.BankTransfer => "Havale / EFT",
        PaymentMethod.Credit => "Kredi",
        PaymentMethod.Mixed => "Karma",
        _ => value.ToString(),
    };

    public static string Text(ExpenseCategory value) => value switch
    {
        ExpenseCategory.Expertise => "Ekspertiz",
        ExpenseCategory.Maintenance => "Bakım / onarım",
        ExpenseCategory.BodyPaint => "Boya / kaporta",
        ExpenseCategory.Tires => "Lastik",
        ExpenseCategory.Notary => "Noter",
        ExpenseCategory.Cleaning => "Temizlik / detay",
        ExpenseCategory.Transport => "Nakliye",
        ExpenseCategory.TaxAndFees => "Vergi / harç",
        ExpenseCategory.Advertising => "Reklam / ilan",
        _ => "Diğer",
    };

    public static string Text(ReminderType value) => value switch
    {
        ReminderType.Inspection => "Muayene",
        ReminderType.TrafficInsurance => "Trafik sigortası",
        ReminderType.Casco => "Kasko",
        ReminderType.Maintenance => "Bakım",
        _ => "Diğer",
    };

    public static string Icon(ReminderType value) => value switch
    {
        ReminderType.Inspection => Icons.Material.Rounded.FactCheck,
        ReminderType.TrafficInsurance => Icons.Material.Rounded.Shield,
        ReminderType.Casco => Icons.Material.Rounded.VerifiedUser,
        ReminderType.Maintenance => Icons.Material.Rounded.Build,
        _ => Icons.Material.Rounded.Event,
    };

    public static string Text(UserRole value) => value switch
    {
        UserRole.SuperAdmin => "Sistem yöneticisi",
        UserRole.GalleryOwner => "Galeri sahibi",
        UserRole.GalleryStaff => "Personel",
        _ => value.ToString(),
    };

    /// <summary>Colour for "days left" chips: overdue red, within a week amber, otherwise neutral.</summary>
    public static Color DueColor(DateTime due) => Format.DaysUntil(due) switch
    {
        < 0 => MudBlazor.Color.Error,
        <= 7 => MudBlazor.Color.Warning,
        _ => MudBlazor.Color.Default,
    };

    public static readonly string[] CommonBrands =
    [
        "Alfa Romeo", "Audi", "BMW", "BYD", "Chery", "Citroën", "Cupra", "Dacia", "DS", "Fiat", "Ford", "Honda", "Hyundai",
        "Jeep", "Kia", "Land Rover", "Lexus", "Mazda", "Mercedes-Benz", "MG", "Mini", "Mitsubishi", "Nissan", "Opel",
        "Peugeot", "Porsche", "Renault", "Seat", "Skoda", "Subaru", "Suzuki", "Tesla", "Togg", "Toyota", "Volkswagen", "Volvo",
    ];
}
