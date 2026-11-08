using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehiclesControl.Data.Context;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Enums;

namespace VehiclesControl.API.Startup;

public static class DatabaseInitializer
{
    /// <summary>
    /// Applies migrations (Database:MigrateOnStartup) and creates the super admin from
    /// Seed:SuperAdminEmail/Password when there are no users yet. With Seed:DemoData, two demo
    /// galleries with stock, customers, sales, expenses and reminders are added as well.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var config = app.Configuration;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(DatabaseInitializer));
        var db = services.GetRequiredService<VehiclesControlContext>();
        var hasher = services.GetRequiredService<IPasswordHasher<User>>();

        if (config.GetValue("Database:MigrateOnStartup", true))
            await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync())
            return;

        var email = config["Seed:SuperAdminEmail"];
        var password = config["Seed:SuperAdminPassword"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("No users exist and Seed:SuperAdminEmail/Password are not set; nobody can sign in yet.");
            return;
        }

        var admin = new User { FirstName = "Sistem", LastName = "Yöneticisi", Email = email.Trim().ToLowerInvariant(), Role = UserRole.SuperAdmin };
        admin.PasswordHash = hasher.HashPassword(admin, password);
        db.Users.Add(admin);
        await db.SaveChangesAsync();
        logger.LogInformation("Seeded super admin {Email}", admin.Email);

        if (config.GetValue("Seed:DemoData", false))
        {
            await DemoData.SeedAsync(db, hasher, config["Seed:DemoPassword"] ?? password);
            logger.LogInformation("Seeded demo galleries");
        }
    }
}
