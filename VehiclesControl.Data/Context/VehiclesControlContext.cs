using Microsoft.EntityFrameworkCore;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Data.Context
{
    public class VehiclesControlContext : DbContext
    {
        public VehiclesControlContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Boat> Boats { get; set; }
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<Vehicle>();
            if (entries.Count() > 0)
            {
                foreach (var item in entries)
                {
                    if (item.State == EntityState.Added)
                    {
                        item.Property(a => a.CreatedDate).CurrentValue = DateTime.Now;
                    }
                    if (item.State == EntityState.Modified)
                    {
                        item.Property(a => a.UpdatedDate).CurrentValue = DateTime.Now;
                    }
                }
            }
            else if (entries.Count() == 0)
            {
                var entries2 = ChangeTracker.Entries<User>();
                foreach (var item in entries2)
                {
                    if (item.State == EntityState.Added)
                    {
                        item.Property(a => a.CreatedDate).CurrentValue = DateTime.Now;
                    }
                    if (item.State == EntityState.Modified)
                    {
                        item.Property(a => a.UpdatedDate).CurrentValue = DateTime.Now;
                    }
                }
            }
            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = "Data Source = (localdb)\\ProjectModels; Initial Catalog=master; Database=VehiclesControls; Integrated Security=True;";
                optionsBuilder.UseSqlServer(connStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });
            }
        }
    }
}
