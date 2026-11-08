using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Customers;

public interface ICustomerService
{
    Task<ApiResponse<PagedResult<CustomerResponse>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponse>> GetAsync(long id, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponse>> CreateAsync(CustomerRequest request, CancellationToken ct = default);
    Task<ApiResponse<CustomerResponse>> UpdateAsync(long id, CustomerRequest request, CancellationToken ct = default);
    Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default);
}

public sealed class CustomerService(IAppDbContext db) : ICustomerService
{
    public async Task<ApiResponse<PagedResult<CustomerResponse>>> GetPagedAsync(PagedQuery query, CancellationToken ct = default)
    {
        var customers = db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var like = Paging.Like(query.Search);
            customers = customers.Where(c =>
                EF.Functions.Like(c.FirstName + " " + c.LastName, like) || EF.Functions.Like(c.Phone, like) ||
                EF.Functions.Like(c.Email!, like) || EF.Functions.Like(c.City!, like));
        }

        customers = query.SortBy?.ToLowerInvariant() switch
        {
            "name" => query.Descending ? customers.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.LastName) : customers.OrderBy(c => c.FirstName).ThenBy(c => c.LastName),
            _ => customers.OrderByDescending(c => c.Id),
        };

        return ApiResponse<PagedResult<CustomerResponse>>.Success(await customers.Select(Projections.Customer).ToPagedAsync(query, ct));
    }

    public async Task<ApiResponse<CustomerResponse>> GetAsync(long id, CancellationToken ct = default) =>
        await db.Customers.Where(c => c.Id == id).Select(Projections.Customer).FirstOrDefaultAsync(ct) is { } customer
            ? ApiResponse<CustomerResponse>.Success(customer)
            : ApiResponse<CustomerResponse>.NotFound("Müşteri bulunamadı.");

    public async Task<ApiResponse<CustomerResponse>> CreateAsync(CustomerRequest request, CancellationToken ct = default)
    {
        var customer = new Customer();
        Apply(request, customer);
        db.Customers.Add(customer);
        await db.SaveChangesAsync(ct);
        return ApiResponse<CustomerResponse>.Created((await GetAsync(customer.Id, ct)).Data!);
    }

    public async Task<ApiResponse<CustomerResponse>> UpdateAsync(long id, CustomerRequest request, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (customer is null)
            return ApiResponse<CustomerResponse>.NotFound("Müşteri bulunamadı.");

        Apply(request, customer);
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (customer is null)
            return ApiResponse<bool>.NotFound("Müşteri bulunamadı.");

        if (await db.Sales.AnyAsync(s => s.CustomerId == id, ct))
            return ApiResponse<bool>.Conflict("Satış kaydı olan müşteri silinemez.");

        db.Customers.Remove(customer);
        await db.SaveChangesAsync(ct);
        return ApiResponse<bool>.Success(true);
    }

    private static void Apply(CustomerRequest r, Customer c)
    {
        c.FirstName = r.FirstName.Trim();
        c.LastName = r.LastName.Trim();
        c.Phone = r.Phone.Trim();
        c.Email = Text.Clean(r.Email)?.ToLowerInvariant();
        c.City = Text.Clean(r.City);
        c.Notes = Text.Clean(r.Notes);
    }
}
