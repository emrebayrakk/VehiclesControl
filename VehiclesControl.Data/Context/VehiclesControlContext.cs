using Microsoft.EntityFrameworkCore;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Data.Context;

/// <summary>
/// Single database for all galleries. Every <see cref="ITenantEntity"/> is filtered to the current
/// gallery and gets its GalleryId stamped on insert, so services never handle tenancy by hand.
/// </summary>
public class VehiclesControlContext(DbContextOptions<VehiclesControlContext> options, ITenantContext tenant)
    : DbContext(options), IAppDbContext
{
    /// <summary>Evaluated per query by the global filters (EF parameterises context members).</summary>
    private long? CurrentGalleryId => tenant.GalleryId;

    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehiclePhoto> VehiclePhotos => Set<VehiclePhoto>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Reminder> Reminders => Set<Reminder>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<decimal>().HavePrecision(18, 2);
        builder.Properties<decimal?>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Gallery>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.City).HasMaxLength(80);
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.Address).HasMaxLength(500);
            e.Property(x => x.TaxNumber).HasMaxLength(20);
        });

        model.Entity<User>(e =>
        {
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30);
            e.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasOne(x => x.Gallery).WithMany(g => g.Users).HasForeignKey(x => x.GalleryId).OnDelete(DeleteBehavior.Restrict);
            e.Ignore(x => x.FullName);
        });

        model.Entity<Vehicle>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.Brand).HasMaxLength(60).IsRequired();
            e.Property(x => x.Model).HasMaxLength(80).IsRequired();
            e.Property(x => x.Trim).HasMaxLength(120);
            e.Property(x => x.Color).HasMaxLength(40).IsRequired();
            e.Property(x => x.Plate).HasMaxLength(15);
            e.Property(x => x.Vin).HasMaxLength(17);
            e.Property(x => x.PaintedParts).HasMaxLength(500);
            e.Property(x => x.ReplacedParts).HasMaxLength(500);
            e.Property(x => x.SellerName).HasMaxLength(150);
            e.Property(x => x.SellerPhone).HasMaxLength(30);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.Property(x => x.PurchaseDate).HasColumnType("date");
            e.Property(x => x.FuelType).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Transmission).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.BodyType).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => new { x.GalleryId, x.Status });
            e.HasIndex(x => new { x.GalleryId, x.Vin }).IsUnique().HasFilter("[Vin] IS NOT NULL");
            e.HasOne<Gallery>().WithMany().HasForeignKey(x => x.GalleryId).OnDelete(DeleteBehavior.Restrict);
            e.Ignore(x => x.Title);
        });

        model.Entity<VehiclePhoto>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.Path).HasMaxLength(300).IsRequired();
            e.HasOne(x => x.Vehicle).WithMany(v => v.Photos).HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Customer>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.City).HasMaxLength(80);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => new { x.GalleryId, x.Phone });
            e.HasOne<Gallery>().WithMany().HasForeignKey(x => x.GalleryId).OnDelete(DeleteBehavior.Restrict);
        });

        model.Entity<Sale>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.SaleDate).HasColumnType("date");
            e.Property(x => x.PaymentMethod).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Notes).HasMaxLength(2000);
            e.HasIndex(x => x.VehicleId).IsUnique();
            e.HasIndex(x => new { x.GalleryId, x.SaleDate });
            e.HasOne(x => x.Vehicle).WithOne(v => v.Sale).HasForeignKey<Sale>(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Customer).WithMany(c => c.Sales).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SoldBy).WithMany().HasForeignKey(x => x.SoldByUserId).OnDelete(DeleteBehavior.SetNull);
        });

        model.Entity<Expense>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.Date).HasColumnType("date");
            e.Property(x => x.Category).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => new { x.GalleryId, x.Date });
            e.HasOne(x => x.Vehicle).WithMany(v => v.Expenses).HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Reminder>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.DueDate).HasColumnType("date");
            e.Property(x => x.CompletedDate).HasColumnType("date");
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Notes).HasMaxLength(500);
            e.HasIndex(x => new { x.GalleryId, x.IsCompleted, x.DueDate });
            e.HasOne(x => x.Vehicle).WithMany(v => v.Reminders).HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<Notification>(e =>
        {
            e.HasQueryFilter(x => x.GalleryId == CurrentGalleryId);
            e.Property(x => x.Title).HasMaxLength(150).IsRequired();
            e.Property(x => x.Message).HasMaxLength(500).IsRequired();
            e.Property(x => x.Link).HasMaxLength(200);
            e.HasIndex(x => new { x.GalleryId, x.IsRead });
            e.HasOne<Gallery>().WithMany().HasForeignKey(x => x.GalleryId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BeforeSave();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        BeforeSave();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void BeforeSave()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is ITenantEntity owned && entry.State == EntityState.Added && owned.GalleryId == 0)
            {
                owned.GalleryId = tenant.GalleryId
                    ?? throw new InvalidOperationException($"Cannot create {entry.Entity.GetType().Name} without a gallery context.");
            }

            if (entry.Entity is ITenantEntity && entry.State == EntityState.Modified && entry.Property(nameof(ITenantEntity.GalleryId)).IsModified)
                throw new InvalidOperationException("GalleryId cannot be changed.");

            if (entry.Entity is IAuditableEntity audited)
            {
                if (entry.State == EntityState.Added)
                {
                    audited.CreatedDate = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(IAuditableEntity.CreatedDate)).IsModified = false;
                    audited.UpdatedDate = now;
                }
            }
        }
    }
}
