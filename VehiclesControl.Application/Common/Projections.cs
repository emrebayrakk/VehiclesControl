using System.Linq.Expressions;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Responses;

namespace VehiclesControl.Application.Common;

/// <summary>EF-translatable projections shared by the services.</summary>
public static class Projections
{
    public static readonly Expression<Func<User, UserResponse>> User = u => new UserResponse
    {
        Id = u.Id,
        FirstName = u.FirstName,
        LastName = u.LastName,
        Email = u.Email,
        Phone = u.Phone,
        Role = u.Role,
        IsActive = u.IsActive,
        GalleryId = u.GalleryId,
        GalleryName = u.Gallery != null ? u.Gallery.Name : null,
        CreatedDate = u.CreatedDate,
        LastLoginAt = u.LastLoginAt,
    };

    public static readonly Expression<Func<Sale, SaleResponse>> Sale = s => new SaleResponse
    {
        Id = s.Id,
        VehicleId = s.VehicleId,
        VehicleTitle = s.Vehicle.Year + " " + s.Vehicle.Brand + " " + s.Vehicle.Model,
        Plate = s.Vehicle.Plate,
        CustomerId = s.CustomerId,
        CustomerName = s.Customer.FirstName + " " + s.Customer.LastName,
        SaleDate = s.SaleDate,
        SalePrice = s.SalePrice,
        Deposit = s.Deposit,
        PaymentMethod = s.PaymentMethod,
        TotalCost = s.Vehicle.PurchasePrice + s.Vehicle.Expenses.Sum(e => e.Amount),
        SoldByName = s.SoldBy != null ? s.SoldBy.FirstName + " " + s.SoldBy.LastName : null,
        Notes = s.Notes,
    };

    public static readonly Expression<Func<Expense, ExpenseResponse>> Expense = e => new ExpenseResponse
    {
        Id = e.Id,
        VehicleId = e.VehicleId,
        VehicleTitle = e.Vehicle != null ? e.Vehicle.Year + " " + e.Vehicle.Brand + " " + e.Vehicle.Model : null,
        Category = e.Category,
        Amount = e.Amount,
        Date = e.Date,
        Description = e.Description,
    };

    public static readonly Expression<Func<Reminder, ReminderResponse>> Reminder = r => new ReminderResponse
    {
        Id = r.Id,
        VehicleId = r.VehicleId,
        VehicleTitle = r.Vehicle.Year + " " + r.Vehicle.Brand + " " + r.Vehicle.Model,
        Plate = r.Vehicle.Plate,
        Type = r.Type,
        DueDate = r.DueDate,
        IsCompleted = r.IsCompleted,
        CompletedDate = r.CompletedDate,
        Notes = r.Notes,
    };

    public static readonly Expression<Func<Customer, CustomerResponse>> Customer = c => new CustomerResponse
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Phone = c.Phone,
        Email = c.Email,
        City = c.City,
        Notes = c.Notes,
        PurchaseCount = c.Sales.Count(),
        TotalSpent = c.Sales.Sum(s => s.SalePrice),
        CreatedDate = c.CreatedDate,
    };
}
