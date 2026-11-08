namespace VehiclesControl.Domain.Enums;

public enum UserRole
{
    SuperAdmin = 0,
    GalleryOwner = 1,
    GalleryStaff = 2,
}

public enum VehicleStatus
{
    InStock = 0,
    Reserved = 1,
    Sold = 2,
}

public enum FuelType
{
    Gasoline = 0,
    Diesel = 1,
    Lpg = 2,
    Hybrid = 3,
    Electric = 4,
}

public enum TransmissionType
{
    Manual = 0,
    Automatic = 1,
    SemiAutomatic = 2,
}

public enum BodyType
{
    Sedan = 0,
    Hatchback = 1,
    StationWagon = 2,
    Suv = 3,
    Coupe = 4,
    Cabrio = 5,
    Mpv = 6,
    Pickup = 7,
    Van = 8,
    Other = 9,
}

public enum PaymentMethod
{
    Cash = 0,
    BankTransfer = 1,
    Credit = 2,
    Mixed = 3,
}

public enum ExpenseCategory
{
    Expertise = 0,
    Maintenance = 1,
    BodyPaint = 2,
    Tires = 3,
    Notary = 4,
    Cleaning = 5,
    Transport = 6,
    TaxAndFees = 7,
    Advertising = 8,
    Other = 9,
}

public enum ReminderType
{
    Inspection = 0,
    TrafficInsurance = 1,
    Casco = 2,
    Maintenance = 3,
    Other = 4,
}

public enum ReminderScope
{
    Upcoming = 0,
    Overdue = 1,
    Completed = 2,
    All = 3,
}
