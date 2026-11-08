using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Galleries;

/// <summary>The signed-in user's own gallery.</summary>
public interface IGalleryService
{
    Task<ApiResponse<GalleryResponse>> GetMineAsync(CancellationToken ct = default);
    Task<ApiResponse<GalleryResponse>> UpdateMineAsync(GalleryRequest request, CancellationToken ct = default);
}

public sealed class GalleryService(IAppDbContext db, ITenantContext tenant) : IGalleryService
{
    public async Task<ApiResponse<GalleryResponse>> GetMineAsync(CancellationToken ct = default)
    {
        var gallery = await db.Galleries
            .Where(g => g.Id == tenant.GalleryId)
            .Select(g => new GalleryResponse
            {
                Id = g.Id,
                Name = g.Name,
                City = g.City,
                Phone = g.Phone,
                Email = g.Email,
                Address = g.Address,
                TaxNumber = g.TaxNumber,
                IsActive = g.IsActive,
                CreatedDate = g.CreatedDate,
                UserCount = g.Users.Count(),
            })
            .FirstOrDefaultAsync(ct);

        return gallery is null ? ApiResponse<GalleryResponse>.NotFound() : ApiResponse<GalleryResponse>.Success(gallery);
    }

    public async Task<ApiResponse<GalleryResponse>> UpdateMineAsync(GalleryRequest request, CancellationToken ct = default)
    {
        var gallery = await db.Galleries.FirstOrDefaultAsync(g => g.Id == tenant.GalleryId, ct);
        if (gallery is null)
            return ApiResponse<GalleryResponse>.NotFound();

        GalleryAdminService.Apply(request, gallery);
        await db.SaveChangesAsync(ct);
        return await GetMineAsync(ct);
    }
}
