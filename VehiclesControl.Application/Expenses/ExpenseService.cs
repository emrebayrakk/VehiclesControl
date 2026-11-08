using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Expenses;

public interface IExpenseService
{
    Task<ApiResponse<ExpenseListResponse>> GetPagedAsync(ExpenseQuery query, CancellationToken ct = default);
    Task<ApiResponse<ExpenseResponse>> CreateAsync(ExpenseRequest request, CancellationToken ct = default);
    Task<ApiResponse<ExpenseResponse>> UpdateAsync(long id, ExpenseRequest request, CancellationToken ct = default);
    Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default);
}

public sealed class ExpenseService(IAppDbContext db) : IExpenseService
{
    public async Task<ApiResponse<ExpenseListResponse>> GetPagedAsync(ExpenseQuery query, CancellationToken ct = default)
    {
        var expenses = db.Expenses.AsNoTracking();
        if (query.VehicleId is { } vehicleId)
            expenses = expenses.Where(e => e.VehicleId == vehicleId);
        if (query.Category is { } category)
            expenses = expenses.Where(e => e.Category == category);
        if (query.From is { } from)
            expenses = expenses.Where(e => e.Date >= from.Date);
        if (query.To is { } to)
            expenses = expenses.Where(e => e.Date <= to.Date);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var like = Paging.Like(query.Search);
            expenses = expenses.Where(e => EF.Functions.Like(e.Description!, like) ||
                (e.Vehicle != null && (EF.Functions.Like(e.Vehicle.Brand + " " + e.Vehicle.Model, like) || EF.Functions.Like(e.Vehicle.Plate!, like))));
        }

        var total = await expenses.SumAsync(e => e.Amount, ct);
        var ordered = query.SortBy?.ToLowerInvariant() == "amount"
            ? (query.Descending ? expenses.OrderByDescending(e => e.Amount) : expenses.OrderBy(e => e.Amount))
            : (query.Descending ? expenses.OrderByDescending(e => e.Date).ThenByDescending(e => e.Id) : expenses.OrderBy(e => e.Date));

        var page = await ordered.Select(Projections.Expense).ToPagedAsync(query, ct);
        return ApiResponse<ExpenseListResponse>.Success(new ExpenseListResponse(page, total));
    }

    public async Task<ApiResponse<ExpenseResponse>> CreateAsync(ExpenseRequest request, CancellationToken ct = default)
    {
        if (request.VehicleId is { } vehicleId && !await db.Vehicles.AnyAsync(v => v.Id == vehicleId, ct))
            return ApiResponse<ExpenseResponse>.NotFound("Araç bulunamadı.");

        var expense = new Expense();
        Apply(request, expense);
        db.Expenses.Add(expense);
        await db.SaveChangesAsync(ct);
        return ApiResponse<ExpenseResponse>.Created(await GetAsync(expense.Id, ct));
    }

    public async Task<ApiResponse<ExpenseResponse>> UpdateAsync(long id, ExpenseRequest request, CancellationToken ct = default)
    {
        var expense = await db.Expenses.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (expense is null)
            return ApiResponse<ExpenseResponse>.NotFound("Masraf bulunamadı.");

        if (request.VehicleId is { } vehicleId && !await db.Vehicles.AnyAsync(v => v.Id == vehicleId, ct))
            return ApiResponse<ExpenseResponse>.NotFound("Araç bulunamadı.");

        Apply(request, expense);
        await db.SaveChangesAsync(ct);
        return ApiResponse<ExpenseResponse>.Success(await GetAsync(id, ct));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var expense = await db.Expenses.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (expense is null)
            return ApiResponse<bool>.NotFound("Masraf bulunamadı.");

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.Success(true);
    }

    private Task<ExpenseResponse> GetAsync(long id, CancellationToken ct) =>
        db.Expenses.Where(e => e.Id == id).Select(Projections.Expense).FirstAsync(ct);

    private static void Apply(ExpenseRequest r, Expense e)
    {
        e.VehicleId = r.VehicleId;
        e.Category = r.Category;
        e.Amount = r.Amount;
        e.Date = (r.Date ?? DateTime.Today).Date;
        e.Description = Text.Clean(r.Description);
    }
}
