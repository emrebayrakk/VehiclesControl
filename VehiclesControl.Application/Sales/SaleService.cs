using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Sales;

public interface ISaleService
{
    Task<ApiResponse<SaleListResponse>> GetPagedAsync(SaleQuery query, CancellationToken ct = default);
    Task<ApiResponse<SaleResponse>> GetAsync(long id, CancellationToken ct = default);
    Task<ApiResponse<SaleResponse>> CreateAsync(SaleRequest request, CancellationToken ct = default);

    /// <summary>Cancels the sale and puts the vehicle back in stock.</summary>
    Task<ApiResponse<bool>> CancelAsync(long id, CancellationToken ct = default);
}

public sealed class SaleService(IAppDbContext db, ITenantContext tenant) : ISaleService
{
    public async Task<ApiResponse<SaleListResponse>> GetPagedAsync(SaleQuery query, CancellationToken ct = default)
    {
        var sales = db.Sales.AsNoTracking();

        if (query.From is { } from)
            sales = sales.Where(s => s.SaleDate >= from.Date);
        if (query.To is { } to)
            sales = sales.Where(s => s.SaleDate <= to.Date);
        if (query.CustomerId is { } customerId)
            sales = sales.Where(s => s.CustomerId == customerId);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var like = Paging.Like(query.Search);
            sales = sales.Where(s =>
                EF.Functions.Like(s.Vehicle.Brand + " " + s.Vehicle.Model, like) || EF.Functions.Like(s.Vehicle.Plate!, like) ||
                EF.Functions.Like(s.Customer.FirstName + " " + s.Customer.LastName, like));
        }

        var revenue = await sales.SumAsync(s => s.SalePrice, ct);
        var purchase = await sales.SumAsync(s => s.Vehicle.PurchasePrice, ct);
        var vehicleIds = sales.Select(s => s.VehicleId);
        var expenses = await db.Expenses.Where(e => e.VehicleId != null && vehicleIds.Contains(e.VehicleId.Value)).SumAsync(e => e.Amount, ct);

        var ordered = query.SortBy?.ToLowerInvariant() == "price"
            ? (query.Descending ? sales.OrderByDescending(s => s.SalePrice) : sales.OrderBy(s => s.SalePrice))
            : (query.Descending ? sales.OrderByDescending(s => s.SaleDate).ThenByDescending(s => s.Id) : sales.OrderBy(s => s.SaleDate));

        var page = await ordered.Select(Projections.Sale).ToPagedAsync(query, ct);
        return ApiResponse<SaleListResponse>.Success(new SaleListResponse(page, new SaleTotals(page.TotalCount, revenue, revenue - purchase - expenses)));
    }

    public async Task<ApiResponse<SaleResponse>> GetAsync(long id, CancellationToken ct = default) =>
        await db.Sales.Where(s => s.Id == id).Select(Projections.Sale).FirstOrDefaultAsync(ct) is { } sale
            ? ApiResponse<SaleResponse>.Success(sale)
            : ApiResponse<SaleResponse>.NotFound("Satış bulunamadı.");

    public async Task<ApiResponse<SaleResponse>> CreateAsync(SaleRequest request, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.Include(v => v.Sale).FirstOrDefaultAsync(v => v.Id == request.VehicleId, ct);
        if (vehicle is null)
            return ApiResponse<SaleResponse>.NotFound("Araç bulunamadı.");

        if (vehicle.Sale is not null || vehicle.Status == VehicleStatus.Sold)
            return ApiResponse<SaleResponse>.Conflict("Bu araç zaten satılmış.");

        if (!await db.Customers.AnyAsync(c => c.Id == request.CustomerId, ct))
            return ApiResponse<SaleResponse>.NotFound("Müşteri bulunamadı.");

        var sale = new Sale
        {
            VehicleId = vehicle.Id,
            CustomerId = request.CustomerId,
            SaleDate = (request.SaleDate ?? DateTime.Today).Date,
            SalePrice = request.SalePrice,
            Deposit = request.Deposit,
            PaymentMethod = request.PaymentMethod,
            Notes = Text.Clean(request.Notes),
            SoldByUserId = tenant.UserId,
        };
        vehicle.Status = VehicleStatus.Sold;
        db.Sales.Add(sale);
        await db.SaveChangesAsync(ct);

        return ApiResponse<SaleResponse>.Created((await GetAsync(sale.Id, ct)).Data!);
    }

    public async Task<ApiResponse<bool>> CancelAsync(long id, CancellationToken ct = default)
    {
        var sale = await db.Sales.Include(s => s.Vehicle).FirstOrDefaultAsync(s => s.Id == id, ct);
        if (sale is null)
            return ApiResponse<bool>.NotFound("Satış bulunamadı.");

        sale.Vehicle.Status = VehicleStatus.InStock;
        db.Sales.Remove(sale);
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.Success(true);
    }
}
