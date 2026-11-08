using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Sales;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;
using VehiclesControl.Tests.Support;

namespace VehiclesControl.Tests.Sales;

public sealed class SaleServiceTests
{
    private readonly TestDb _db = new();

    private async Task<(long VehicleId, long CustomerId)> SeedAsync()
    {
        await using var system = _db.System();
        system.Galleries.Add(new Gallery { Id = 1, Name = "A" });
        var vehicle = TestDb.Vehicle(1, buy: 500_000);
        var customer = TestDb.Customer(1);
        system.AddRange(vehicle, customer);
        await system.SaveChangesAsync();
        system.Expenses.AddRange(
            new Expense { GalleryId = 1, VehicleId = vehicle.Id, Amount = 10_000, Date = DateTime.Today },
            new Expense { GalleryId = 1, VehicleId = vehicle.Id, Amount = 5_000, Date = DateTime.Today },
            new Expense { GalleryId = 1, Amount = 99_999, Date = DateTime.Today, Description = "General, not part of the car cost" });
        await system.SaveChangesAsync();
        return (vehicle.Id, customer.Id);
    }

    [Fact]
    public async Task Sale_marks_vehicle_sold_and_profit_subtracts_purchase_and_vehicle_expenses()
    {
        var (vehicleId, customerId) = await SeedAsync();
        await using var ctx = _db.As(1, userId: 7);
        var service = new SaleService(ctx, new TestTenant { GalleryId = 1, UserId = 7 });

        var result = await service.CreateAsync(new SaleRequest { VehicleId = vehicleId, CustomerId = customerId, SalePrice = 560_000 });

        result.Code.Should().Be(ResultCode.Created);
        result.Data!.TotalCost.Should().Be(515_000);
        result.Data.Profit.Should().Be(45_000);
        (await ctx.Vehicles.SingleAsync()).Status.Should().Be(VehicleStatus.Sold);
        (await ctx.Sales.SingleAsync()).SoldByUserId.Should().Be(7);
    }

    [Fact]
    public async Task A_vehicle_can_only_be_sold_once()
    {
        var (vehicleId, customerId) = await SeedAsync();
        await using var ctx = _db.As(1);
        var service = new SaleService(ctx, new TestTenant { GalleryId = 1 });
        var request = new SaleRequest { VehicleId = vehicleId, CustomerId = customerId, SalePrice = 1 };

        await service.CreateAsync(request);
        var second = await service.CreateAsync(request);

        second.Code.Should().Be(ResultCode.Conflict);
    }

    [Fact]
    public async Task Cancelling_a_sale_puts_the_vehicle_back_in_stock()
    {
        var (vehicleId, customerId) = await SeedAsync();
        await using var ctx = _db.As(1);
        var service = new SaleService(ctx, new TestTenant { GalleryId = 1 });
        var sale = await service.CreateAsync(new SaleRequest { VehicleId = vehicleId, CustomerId = customerId, SalePrice = 1 });

        var cancel = await service.CancelAsync(sale.Data!.Id);

        cancel.Status.Should().BeTrue();
        (await ctx.Sales.CountAsync()).Should().Be(0);
        (await ctx.Vehicles.SingleAsync()).Status.Should().Be(VehicleStatus.InStock);
    }
}
