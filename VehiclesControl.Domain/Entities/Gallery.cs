namespace VehiclesControl.Domain.Entities;

/// <summary>A dealership (tenant). Created by the super admin.</summary>
public class Gallery : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? TaxNumber { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<User> Users { get; set; } = [];
}
