using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Common;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;
using VehiclesControl.Domain.Requests;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Vehicles;

public interface IVehicleService
{
    Task<ApiResponse<PagedResult<VehicleListItem>>> GetPagedAsync(VehicleQuery query, CancellationToken ct = default);
    Task<ApiResponse<List<VehicleOption>>> GetOptionsAsync(string? search, bool availableOnly, CancellationToken ct = default);
    Task<ApiResponse<List<string>>> GetBrandsAsync(CancellationToken ct = default);
    Task<ApiResponse<VehicleDetail>> GetAsync(long id, CancellationToken ct = default);
    Task<ApiResponse<VehicleDetail>> CreateAsync(VehicleRequest request, CancellationToken ct = default);
    Task<ApiResponse<VehicleDetail>> UpdateAsync(long id, VehicleRequest request, CancellationToken ct = default);
    Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default);
    Task<ApiResponse<VehicleDetail>> SetStatusAsync(long id, VehicleStatus status, CancellationToken ct = default);
    Task<ApiResponse<List<PhotoResponse>>> AddPhotosAsync(long id, IReadOnlyList<UploadedFile> files, CancellationToken ct = default);
    Task<ApiResponse<List<PhotoResponse>>> DeletePhotoAsync(long id, long photoId, CancellationToken ct = default);
    Task<ApiResponse<List<PhotoResponse>>> SetCoverAsync(long id, long photoId, CancellationToken ct = default);
}

public sealed class VehicleService(IAppDbContext db, IFileStorage storage, ITenantContext tenant) : IVehicleService
{
    public const int MaxPhotos = 20;
    public const long MaxPhotoBytes = 8 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public async Task<ApiResponse<PagedResult<VehicleListItem>>> GetPagedAsync(VehicleQuery query, CancellationToken ct = default)
    {
        var vehicles = db.Vehicles.AsNoTracking();

        if (query.Status is { } status)
            vehicles = vehicles.Where(v => v.Status == status);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var like = Paging.Like(query.Search);
            vehicles = vehicles.Where(v =>
                EF.Functions.Like(v.Brand, like) || EF.Functions.Like(v.Model, like) || EF.Functions.Like(v.Trim!, like) ||
                EF.Functions.Like(v.Plate!, like) || EF.Functions.Like(v.Vin!, like) || EF.Functions.Like(v.Color, like));
        }

        vehicles = (query.SortBy?.ToLowerInvariant(), query.Descending) switch
        {
            ("listprice", true) => vehicles.OrderByDescending(v => v.ListPrice),
            ("listprice", false) => vehicles.OrderBy(v => v.ListPrice),
            ("year", true) => vehicles.OrderByDescending(v => v.Year),
            ("year", false) => vehicles.OrderBy(v => v.Year),
            ("kilometers", true) => vehicles.OrderByDescending(v => v.Kilometers),
            ("kilometers", false) => vehicles.OrderBy(v => v.Kilometers),
            ("brand", true) => vehicles.OrderByDescending(v => v.Brand).ThenByDescending(v => v.Model),
            ("brand", false) => vehicles.OrderBy(v => v.Brand).ThenBy(v => v.Model),
            ("purchasedate", false) => vehicles.OrderBy(v => v.PurchaseDate),
            (_, false) => vehicles.OrderBy(v => v.Id),
            _ => vehicles.OrderByDescending(v => v.PurchaseDate).ThenByDescending(v => v.Id),
        };

        var page = await vehicles.Select(v => new VehicleListItem
        {
            Id = v.Id,
            Brand = v.Brand,
            Model = v.Model,
            Trim = v.Trim,
            Year = v.Year,
            Kilometers = v.Kilometers,
            FuelType = v.FuelType,
            Transmission = v.Transmission,
            Color = v.Color,
            Plate = v.Plate,
            PurchasePrice = v.PurchasePrice,
            TotalExpenses = v.Expenses.Sum(e => e.Amount),
            ListPrice = v.ListPrice,
            Status = v.Status,
            CoverPhotoUrl = v.Photos.OrderByDescending(p => p.IsCover).ThenBy(p => p.SortOrder).Select(p => p.Path).FirstOrDefault(),
            PurchaseDate = v.PurchaseDate,
            CreatedDate = v.CreatedDate,
        }).ToPagedAsync(query, ct);

        foreach (var item in page.Items)
            item.CoverPhotoUrl = item.CoverPhotoUrl is null ? null : storage.GetUrl(item.CoverPhotoUrl);

        return ApiResponse<PagedResult<VehicleListItem>>.Success(page);
    }

    public async Task<ApiResponse<List<VehicleOption>>> GetOptionsAsync(string? search, bool availableOnly, CancellationToken ct = default)
    {
        var vehicles = db.Vehicles.AsNoTracking();
        if (availableOnly)
            vehicles = vehicles.Where(v => v.Status != VehicleStatus.Sold);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var like = Paging.Like(search);
            vehicles = vehicles.Where(v => EF.Functions.Like(v.Brand + " " + v.Model, like) || EF.Functions.Like(v.Plate!, like));
        }

        var options = await vehicles
            .OrderBy(v => v.Brand).ThenBy(v => v.Model)
            .Take(25)
            .Select(v => new VehicleOption(v.Id, v.Year + " " + v.Brand + " " + v.Model, v.Plate, v.Status, v.ListPrice))
            .ToListAsync(ct);
        return ApiResponse<List<VehicleOption>>.Success(options);
    }

    public async Task<ApiResponse<List<string>>> GetBrandsAsync(CancellationToken ct = default) =>
        ApiResponse<List<string>>.Success(await db.Vehicles.Select(v => v.Brand).Distinct().OrderBy(b => b).ToListAsync(ct));

    public async Task<ApiResponse<VehicleDetail>> GetAsync(long id, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.AsNoTracking()
            .Include(v => v.Photos)
            .Include(v => v.Sale).ThenInclude(s => s!.Customer)
            .Include(v => v.Sale).ThenInclude(s => s!.SoldBy)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

        if (vehicle is null)
            return ApiResponse<VehicleDetail>.NotFound("Araç bulunamadı.");

        var detail = ToDetail(vehicle);
        detail.Expenses = await db.Expenses.Where(e => e.VehicleId == id).OrderByDescending(e => e.Date).Select(Projections.Expense).ToListAsync(ct);
        detail.Reminders = await db.Reminders.Where(r => r.VehicleId == id).OrderBy(r => r.IsCompleted).ThenBy(r => r.DueDate).Select(Projections.Reminder).ToListAsync(ct);
        return ApiResponse<VehicleDetail>.Success(detail);
    }

    public async Task<ApiResponse<VehicleDetail>> CreateAsync(VehicleRequest request, CancellationToken ct = default)
    {
        if (await VinTakenAsync(request.Vin, null, ct))
            return ApiResponse<VehicleDetail>.Conflict("Bu şasi numarasıyla kayıtlı bir araç zaten var.");

        var vehicle = new Vehicle { Status = VehicleStatus.InStock };
        Apply(request, vehicle);
        db.Vehicles.Add(vehicle);
        await db.SaveChangesAsync(ct);

        var created = await GetAsync(vehicle.Id, ct);
        return ApiResponse<VehicleDetail>.Created(created.Data!);
    }

    public async Task<ApiResponse<VehicleDetail>> UpdateAsync(long id, VehicleRequest request, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vehicle is null)
            return ApiResponse<VehicleDetail>.NotFound("Araç bulunamadı.");

        if (await VinTakenAsync(request.Vin, id, ct))
            return ApiResponse<VehicleDetail>.Conflict("Bu şasi numarasıyla kayıtlı başka bir araç var.");

        Apply(request, vehicle);
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<ApiResponse<bool>> DeleteAsync(long id, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.Include(v => v.Photos).Include(v => v.Sale).FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vehicle is null)
            return ApiResponse<bool>.NotFound("Araç bulunamadı.");

        if (vehicle.Sale is not null)
            return ApiResponse<bool>.Conflict("Satışı yapılmış araç silinemez. Önce satışı iptal edin.");

        var paths = vehicle.Photos.Select(p => p.Path).ToList();
        db.Vehicles.Remove(vehicle);
        await db.SaveChangesAsync(ct);

        foreach (var path in paths)
            await storage.DeleteAsync(path);

        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<VehicleDetail>> SetStatusAsync(long id, VehicleStatus status, CancellationToken ct = default)
    {
        if (status == VehicleStatus.Sold)
            return ApiResponse<VehicleDetail>.BadRequest("Aracı satıldı olarak işaretlemek için satış kaydı oluşturun.");

        var vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vehicle is null)
            return ApiResponse<VehicleDetail>.NotFound("Araç bulunamadı.");

        if (vehicle.Status == VehicleStatus.Sold)
            return ApiResponse<VehicleDetail>.Conflict("Satılmış aracın durumu değiştirilemez. Önce satışı iptal edin.");

        vehicle.Status = status;
        await db.SaveChangesAsync(ct);
        return await GetAsync(id, ct);
    }

    public async Task<ApiResponse<List<PhotoResponse>>> AddPhotosAsync(long id, IReadOnlyList<UploadedFile> files, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.Include(v => v.Photos).FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vehicle is null)
            return ApiResponse<List<PhotoResponse>>.NotFound("Araç bulunamadı.");

        if (files.Count == 0)
            return ApiResponse<List<PhotoResponse>>.BadRequest("Fotoğraf seçilmedi.");

        if (vehicle.Photos.Count + files.Count > MaxPhotos)
            return ApiResponse<List<PhotoResponse>>.BadRequest($"Bir araca en fazla {MaxPhotos} fotoğraf eklenebilir.");

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return ApiResponse<List<PhotoResponse>>.BadRequest($"{file.FileName}: yalnızca JPG, PNG veya WEBP yüklenebilir.");
            if (file.Length is 0 or > MaxPhotoBytes)
                return ApiResponse<List<PhotoResponse>>.BadRequest($"{file.FileName}: dosya boyutu 8 MB'ı geçemez.");
        }

        var order = vehicle.Photos.Count == 0 ? 0 : vehicle.Photos.Max(p => p.SortOrder) + 1;
        var hasCover = vehicle.Photos.Any(p => p.IsCover);
        foreach (var file in files)
        {
            var path = await storage.SaveAsync(file.Content, $"{tenant.GalleryId}/{vehicle.Id}", Path.GetExtension(file.FileName).ToLowerInvariant(), ct);
            vehicle.Photos.Add(new VehiclePhoto { Path = path, SortOrder = order++, IsCover = !hasCover });
            hasCover = true;
        }

        await db.SaveChangesAsync(ct);
        return ApiResponse<List<PhotoResponse>>.Success(ToPhotos(vehicle.Photos));
    }

    public async Task<ApiResponse<List<PhotoResponse>>> DeletePhotoAsync(long id, long photoId, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.Include(v => v.Photos).FirstOrDefaultAsync(v => v.Id == id, ct);
        var photo = vehicle?.Photos.FirstOrDefault(p => p.Id == photoId);
        if (vehicle is null || photo is null)
            return ApiResponse<List<PhotoResponse>>.NotFound("Fotoğraf bulunamadı.");

        vehicle.Photos.Remove(photo);
        db.VehiclePhotos.Remove(photo);
        if (photo.IsCover && vehicle.Photos.OrderBy(p => p.SortOrder).FirstOrDefault() is { } next)
            next.IsCover = true;

        await db.SaveChangesAsync(ct);
        await storage.DeleteAsync(photo.Path);
        return ApiResponse<List<PhotoResponse>>.Success(ToPhotos(vehicle.Photos));
    }

    public async Task<ApiResponse<List<PhotoResponse>>> SetCoverAsync(long id, long photoId, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.Include(v => v.Photos).FirstOrDefaultAsync(v => v.Id == id, ct);
        if (vehicle is null || vehicle.Photos.All(p => p.Id != photoId))
            return ApiResponse<List<PhotoResponse>>.NotFound("Fotoğraf bulunamadı.");

        foreach (var photo in vehicle.Photos)
            photo.IsCover = photo.Id == photoId;

        await db.SaveChangesAsync(ct);
        return ApiResponse<List<PhotoResponse>>.Success(ToPhotos(vehicle.Photos));
    }

    private Task<bool> VinTakenAsync(string? vin, long? exceptId, CancellationToken ct)
    {
        var normalized = Text.Clean(vin)?.ToUpperInvariant();
        return normalized is null
            ? Task.FromResult(false)
            : db.Vehicles.AnyAsync(v => v.Vin == normalized && v.Id != exceptId, ct);
    }

    private static void Apply(VehicleRequest r, Vehicle v)
    {
        v.Brand = r.Brand.Trim();
        v.Model = r.Model.Trim();
        v.Trim = Text.Clean(r.Trim);
        v.Year = r.Year;
        v.Kilometers = r.Kilometers;
        v.FuelType = r.FuelType;
        v.Transmission = r.Transmission;
        v.BodyType = r.BodyType;
        v.Color = r.Color.Trim();
        v.EngineVolume = r.EngineVolume;
        v.EnginePower = r.EnginePower;
        v.Plate = Text.Plate(r.Plate);
        v.Vin = Text.Clean(r.Vin)?.ToUpperInvariant();
        v.DamageRecordAmount = r.DamageRecordAmount;
        v.PaintedParts = Text.Clean(r.PaintedParts);
        v.ReplacedParts = Text.Clean(r.ReplacedParts);
        v.PurchaseDate = (r.PurchaseDate ?? DateTime.Today).Date;
        v.PurchasePrice = r.PurchasePrice;
        v.SellerName = Text.Clean(r.SellerName);
        v.SellerPhone = Text.Clean(r.SellerPhone);
        v.ListPrice = r.ListPrice;
        v.Notes = Text.Clean(r.Notes);
    }

    private List<PhotoResponse> ToPhotos(IEnumerable<VehiclePhoto> photos) =>
        photos.OrderByDescending(p => p.IsCover).ThenBy(p => p.SortOrder)
            .Select(p => new PhotoResponse(p.Id, storage.GetUrl(p.Path), p.IsCover))
            .ToList();

    private VehicleDetail ToDetail(Vehicle v) => new()
    {
        Id = v.Id,
        Brand = v.Brand,
        Model = v.Model,
        Trim = v.Trim,
        Year = v.Year,
        Kilometers = v.Kilometers,
        FuelType = v.FuelType,
        Transmission = v.Transmission,
        BodyType = v.BodyType,
        Color = v.Color,
        EngineVolume = v.EngineVolume,
        EnginePower = v.EnginePower,
        Plate = v.Plate,
        Vin = v.Vin,
        DamageRecordAmount = v.DamageRecordAmount,
        PaintedParts = v.PaintedParts,
        ReplacedParts = v.ReplacedParts,
        PurchaseDate = v.PurchaseDate,
        PurchasePrice = v.PurchasePrice,
        SellerName = v.SellerName,
        SellerPhone = v.SellerPhone,
        ListPrice = v.ListPrice,
        Status = v.Status,
        Notes = v.Notes,
        CreatedDate = v.CreatedDate,
        Photos = ToPhotos(v.Photos),
        Sale = v.Sale is { } s
            ? new SaleSummary
            {
                Id = s.Id,
                CustomerId = s.CustomerId,
                CustomerName = $"{s.Customer.FirstName} {s.Customer.LastName}",
                CustomerPhone = s.Customer.Phone,
                SaleDate = s.SaleDate,
                SalePrice = s.SalePrice,
                Deposit = s.Deposit,
                PaymentMethod = s.PaymentMethod,
                SoldByName = s.SoldBy?.FullName,
                Notes = s.Notes,
            }
            : null,
    };
}
