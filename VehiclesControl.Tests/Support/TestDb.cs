using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;

namespace VehiclesControl.Tests.Support;

public sealed class TestTenant : ITenantContext
{
    public long? GalleryId { get; set; }
    public long? UserId { get; set; }
    public UserRole? Role { get; set; }
}

/// <summary>One in-memory database per test; open a context "as" any gallery.</summary>
public sealed class TestDb
{
    private readonly string _name = Guid.NewGuid().ToString("N");

    public VehiclesControlContext As(long? galleryId, long? userId = null, UserRole? role = null) =>
        new(new DbContextOptionsBuilder<VehiclesControlContext>().UseInMemoryDatabase(_name).Options,
            new TestTenant { GalleryId = galleryId, UserId = userId, Role = role ?? (galleryId is null ? UserRole.SuperAdmin : UserRole.GalleryOwner) });

    public VehiclesControlContext System() => As(null);

    public static Vehicle Vehicle(long galleryId, string brand = "Renault", decimal buy = 500_000, decimal list = 560_000) => new()
    {
        GalleryId = galleryId,
        Brand = brand,
        Model = "Clio",
        Year = 2021,
        Kilometers = 40_000,
        Color = "Beyaz",
        PurchaseDate = DateTime.Today.AddDays(-10),
        PurchasePrice = buy,
        ListPrice = list,
    };

    public static Customer Customer(long galleryId) => new() { GalleryId = galleryId, FirstName = "Ali", LastName = "Veli", Phone = "0555" };
}
