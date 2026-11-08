using FakeItEasy;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Sales;
using VehiclesControl.Application.Vehicles;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;
using VehiclesControl.Tests.Support;

namespace VehiclesControl.Tests.Tenancy;

public sealed class TenantIsolationTests
{
    private readonly TestDb _db = new();
    private long _vehicleA, _vehicleB, _customerA, _customerB;

    private async Task SeedAsync()
    {
        await using var system = _db.System();
        system.Galleries.AddRange(new Gallery { Id = 1, Name = "A" }, new Gallery { Id = 2, Name = "B" });
        var va = TestDb.Vehicle(1, "Renault");
        var vb = TestDb.Vehicle(2, "BMW");
        var ca = TestDb.Customer(1);
        var cb = TestDb.Customer(2);
        system.AddRange(va, vb, ca, cb);
        await system.SaveChangesAsync();
        (_vehicleA, _vehicleB, _customerA, _customerB) = (va.Id, vb.Id, ca.Id, cb.Id);
    }

    [Fact]
    public async Task Each_gallery_only_sees_its_own_rows()
    {
        await SeedAsync();

        await using var a = _db.As(1);
        await using var b = _db.As(2);

        (await a.Vehicles.Select(v => v.Brand).ToListAsync()).Should().Equal("Renault");
        (await b.Vehicles.Select(v => v.Brand).ToListAsync()).Should().Equal("BMW");
        (await a.Customers.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Super_admin_context_sees_no_tenant_rows_without_ignoring_filters()
    {
        await SeedAsync();
        await using var system = _db.System();

        (await system.Vehicles.CountAsync()).Should().Be(0);
        (await system.Vehicles.IgnoreQueryFilters().CountAsync()).Should().Be(2);
    }

    [Fact]
    public async Task Reading_another_gallerys_vehicle_returns_404()
    {
        await SeedAsync();
        await using var a = _db.As(1);
        var service = new VehicleService(a, A.Fake<IFileStorage>(), new TestTenant { GalleryId = 1 });

        var result = await service.GetAsync(_vehicleB);

        result.Code.Should().Be(ResultCode.NotFound);
    }

    [Fact]
    public async Task Cannot_sell_another_gallerys_vehicle_or_to_their_customer()
    {
        await SeedAsync();
        await using var a = _db.As(1);
        var sales = new SaleService(a, new TestTenant { GalleryId = 1 });

        (await sales.CreateAsync(new SaleRequest { VehicleId = _vehicleB, CustomerId = _customerA, SalePrice = 1 })).Code.Should().Be(ResultCode.NotFound);
        (await sales.CreateAsync(new SaleRequest { VehicleId = _vehicleA, CustomerId = _customerB, SalePrice = 1 })).Code.Should().Be(ResultCode.NotFound);
    }

    [Fact]
    public async Task New_rows_get_the_current_gallery_id()
    {
        await SeedAsync();
        await using (var a = _db.As(1))
        {
            var service = new VehicleService(a, A.Fake<IFileStorage>(), new TestTenant { GalleryId = 1 });
            var created = await service.CreateAsync(new VehicleRequest
            {
                Brand = "Fiat", Model = "Egea", Year = 2020, Color = "Gri", PurchasePrice = 1, ListPrice = 2,
            });
            created.Code.Should().Be(ResultCode.Created);
        }

        await using var system = _db.System();
        (await system.Vehicles.IgnoreQueryFilters().SingleAsync(v => v.Brand == "Fiat")).GalleryId.Should().Be(1);
    }

    [Fact]
    public async Task Creating_tenant_rows_without_a_gallery_throws()
    {
        await using var system = _db.System();
        system.Customers.Add(new Customer { FirstName = "x", LastName = "y", Phone = "1" });

        var act = () => system.SaveChangesAsync();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
