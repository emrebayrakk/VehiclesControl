using Microsoft.EntityFrameworkCore;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Common;

public static class Paging
{
    public const int MaxPageSize = 100;

    public static async Task<PagedResult<T>> ToPagedAsync<T>(this IQueryable<T> query, PagedQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);
        var page = Math.Max(1, request.Page);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<T>(items, total, page, pageSize);
    }

    /// <summary>Escapes LIKE wildcards so user input is matched literally.</summary>
    public static string Like(string term) =>
        "%" + term.Trim().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]") + "%";
}
