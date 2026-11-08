using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Galleries;

/// <summary>Super-admin operations across all galleries. Bypasses tenant filters on purpose.</summary>
public interface IGalleryAdminService
{
    Task<ApiResponse<AdminSummaryResponse>> GetSummaryAsync(CancellationToken ct = default);
    Task<ApiResponse<List<GalleryResponse>>> GetAllAsync(string? search, CancellationToken ct = default);
    Task<ApiResponse<GalleryResponse>> CreateAsync(CreateGalleryRequest request, CancellationToken ct = default);
    Task<ApiResponse<GalleryResponse>> UpdateAsync(long id, GalleryRequest request, CancellationToken ct = default);
    Task<ApiResponse<GalleryResponse>> SetActiveAsync(long id, bool isActive, CancellationToken ct = default);
}

public sealed class GalleryAdminService(IAppDbContext db, IPasswordHasher<User> hasher) : IGalleryAdminService
{
    public async Task<ApiResponse<AdminSummaryResponse>> GetSummaryAsync(CancellationToken ct = default) =>
        ApiResponse<AdminSummaryResponse>.Success(new AdminSummaryResponse(
            await db.Galleries.CountAsync(ct),
            await db.Galleries.CountAsync(g => g.IsActive, ct),
            await db.Users.CountAsync(u => u.GalleryId != null, ct),
            await db.Vehicles.IgnoreQueryFilters().CountAsync(ct),
            await db.Sales.IgnoreQueryFilters().CountAsync(ct)));

    public async Task<ApiResponse<List<GalleryResponse>>> GetAllAsync(string? search, CancellationToken ct = default)
    {
        var query = db.Galleries.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var like = Paging.Like(search);
            query = query.Where(g => EF.Functions.Like(g.Name, like) || EF.Functions.Like(g.City!, like));
        }

        return ApiResponse<List<GalleryResponse>>.Success(await Project(query.OrderBy(g => g.Name)).ToListAsync(ct));
    }

    public async Task<ApiResponse<GalleryResponse>> CreateAsync(CreateGalleryRequest request, CancellationToken ct = default)
    {
        var email = Text.Email(request.OwnerEmail);
        if (await db.Users.AnyAsync(u => u.Email == email, ct))
            return ApiResponse<GalleryResponse>.Conflict("Yönetici e-posta adresi zaten kayıtlı.");

        var gallery = new Gallery();
        Apply(request.Gallery, gallery);

        var owner = new User
        {
            Gallery = gallery,
            FirstName = request.OwnerFirstName.Trim(),
            LastName = request.OwnerLastName.Trim(),
            Email = email,
            Phone = Text.Clean(request.OwnerPhone),
            Role = UserRole.GalleryOwner,
        };
        owner.PasswordHash = hasher.HashPassword(owner, request.OwnerPassword);

        db.Galleries.Add(gallery);
        db.Users.Add(owner);
        await db.SaveChangesAsync(ct);

        return ApiResponse<GalleryResponse>.Created(await GetAsync(gallery.Id, ct));
    }

    public async Task<ApiResponse<GalleryResponse>> UpdateAsync(long id, GalleryRequest request, CancellationToken ct = default)
    {
        var gallery = await db.Galleries.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (gallery is null)
            return ApiResponse<GalleryResponse>.NotFound();

        Apply(request, gallery);
        await db.SaveChangesAsync(ct);
        return ApiResponse<GalleryResponse>.Success(await GetAsync(id, ct));
    }

    public async Task<ApiResponse<GalleryResponse>> SetActiveAsync(long id, bool isActive, CancellationToken ct = default)
    {
        var gallery = await db.Galleries.FirstOrDefaultAsync(g => g.Id == id, ct);
        if (gallery is null)
            return ApiResponse<GalleryResponse>.NotFound();

        gallery.IsActive = isActive;
        await db.SaveChangesAsync(ct);
        return ApiResponse<GalleryResponse>.Success(await GetAsync(id, ct));
    }

    internal static void Apply(GalleryRequest request, Gallery gallery)
    {
        gallery.Name = request.Name.Trim();
        gallery.City = Text.Clean(request.City);
        gallery.Phone = Text.Clean(request.Phone);
        gallery.Email = Text.Clean(request.Email)?.ToLowerInvariant();
        gallery.Address = Text.Clean(request.Address);
        gallery.TaxNumber = Text.Clean(request.TaxNumber);
    }

    private Task<GalleryResponse> GetAsync(long id, CancellationToken ct) =>
        Project(db.Galleries.Where(g => g.Id == id)).FirstAsync(ct);

    private IQueryable<GalleryResponse> Project(IQueryable<Gallery> query) => query.Select(g => new GalleryResponse
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
        VehicleCount = db.Vehicles.IgnoreQueryFilters().Count(v => v.GalleryId == g.Id),
        SoldCount = db.Sales.IgnoreQueryFilters().Count(s => s.GalleryId == g.Id),
        OwnerName = g.Users.Where(u => u.Role == UserRole.GalleryOwner).OrderBy(u => u.Id).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
        OwnerEmail = g.Users.Where(u => u.Role == UserRole.GalleryOwner).OrderBy(u => u.Id).Select(u => u.Email).FirstOrDefault(),
    });
}
