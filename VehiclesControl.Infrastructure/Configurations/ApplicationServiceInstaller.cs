using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VehiclesControl.Application.Abstractions;
using VehiclesControl.Application.Authentication;
using VehiclesControl.Application.Customers;
using VehiclesControl.Application.Dashboard;
using VehiclesControl.Application.Expenses;
using VehiclesControl.Application.Galleries;
using VehiclesControl.Application.Notifications;
using VehiclesControl.Application.Reminders;
using VehiclesControl.Application.Sales;
using VehiclesControl.Application.Users;
using VehiclesControl.Application.Validation;
using VehiclesControl.Application.Vehicles;
using VehiclesControl.Domain.Entities;

namespace VehiclesControl.Infrastructure.Configurations;

public sealed class ApplicationServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IGalleryAdminService, GalleryAdminService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ReminderNotificationGenerator>();

        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>(ServiceLifetime.Singleton, includeInternalTypes: true);
    }
}
