using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Dashboard;

public interface IDashboardService
{
    Task<ApiResponse<DashboardResponse>> GetAsync(CancellationToken ct = default);
}

public sealed class DashboardService(IAppDbContext db, TimeProvider clock) : IDashboardService
{
    private const int Months = 6;

    public async Task<ApiResponse<DashboardResponse>> GetAsync(CancellationToken ct = default)
    {
        var today = clock.GetLocalNow().Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var chartStart = monthStart.AddMonths(-(Months - 1));

        var unsold = db.Vehicles.Where(v => v.Status != VehicleStatus.Sold);
        var purchaseDates = await unsold.Select(v => v.PurchaseDate).ToListAsync(ct);

        // Sales of the chart window with their cost; month figures are computed from the same list.
        var sales = await db.Sales
            .Where(s => s.SaleDate >= chartStart)
            .Select(s => new { s.SaleDate, s.SalePrice, Cost = s.Vehicle.PurchasePrice + s.Vehicle.Expenses.Sum(e => e.Amount) })
            .ToListAsync(ct);

        var monthly = Enumerable.Range(0, Months)
            .Select(i => chartStart.AddMonths(i))
            .Select(m =>
            {
                var inMonth = sales.Where(s => s.SaleDate.Year == m.Year && s.SaleDate.Month == m.Month).ToList();
                return new MonthlySalesPoint(m.Year, m.Month, inMonth.Count, inMonth.Sum(s => s.SalePrice), inMonth.Sum(s => s.SalePrice - s.Cost));
            })
            .ToList();

        var current = monthly[^1];
        var dashboard = new DashboardResponse
        {
            InStockCount = await db.Vehicles.CountAsync(v => v.Status == VehicleStatus.InStock, ct),
            ReservedCount = await db.Vehicles.CountAsync(v => v.Status == VehicleStatus.Reserved, ct),
            StockListValue = await unsold.SumAsync(v => v.ListPrice, ct),
            StockCost = await unsold.SumAsync(v => v.PurchasePrice, ct)
                        + await db.Expenses.Where(e => e.Vehicle != null && e.Vehicle.Status != VehicleStatus.Sold).SumAsync(e => e.Amount, ct),
            SalesThisMonth = current.Count,
            RevenueThisMonth = current.Revenue,
            ProfitThisMonth = current.Profit,
            ExpensesThisMonth = await db.Expenses.Where(e => e.Date >= monthStart).SumAsync(e => e.Amount, ct),
            AverageDaysInStock = purchaseDates.Count == 0 ? 0 : Math.Round(purchaseDates.Average(d => (today - d.Date).TotalDays), 1),
            CustomerCount = await db.Customers.CountAsync(ct),
            MonthlySales = monthly,
            UpcomingReminders = await db.Reminders
                .Where(r => !r.IsCompleted && r.DueDate <= today.AddDays(30))
                .OrderBy(r => r.DueDate)
                .Take(6)
                .Select(Projections.Reminder)
                .ToListAsync(ct),
            RecentSales = await db.Sales
                .OrderByDescending(s => s.SaleDate).ThenByDescending(s => s.Id)
                .Take(5)
                .Select(Projections.Sale)
                .ToListAsync(ct),
        };

        return ApiResponse<DashboardResponse>.Success(dashboard);
    }
}
