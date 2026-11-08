using Microsoft.EntityFrameworkCore;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Application.Abstractions;

/// <summary>
/// The parts of the EF Core context the services use. Tenant-owned sets are already filtered to
/// the current gallery; call IgnoreQueryFilters() only in super-admin or background code.
/// </summary>
public interface IAppDbContext
{
    DbSet<Gallery> Galleries { get; }
    DbSet<User> Users { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<VehiclePhoto> VehiclePhotos { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Sale> Sales { get; }
    DbSet<Expense> Expenses { get; }
    DbSet<Reminder> Reminders { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
