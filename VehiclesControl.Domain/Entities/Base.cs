namespace VehiclesControl.Domain.Entities;

public interface IAuditableEntity
{
    DateTime CreatedDate { get; set; }
    DateTime? UpdatedDate { get; set; }
}

/// <summary>Rows that belong to one gallery. The DbContext filters and stamps <see cref="GalleryId"/> automatically.</summary>
public interface ITenantEntity
{
    long GalleryId { get; set; }
}

public abstract class Entity : IAuditableEntity
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public abstract class TenantEntity : Entity, ITenantEntity
{
    public long GalleryId { get; set; }
}
