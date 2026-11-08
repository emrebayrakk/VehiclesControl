namespace VehiclesControl.Application.Authorization;

public static class AppClaims
{
    public const string GalleryId = "gallery_id";
    public const string GalleryName = "gallery_name";
    public const string Role = "role";
}

public static class AppPolicies
{
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>Any active user that belongs to a gallery (owner or staff).</summary>
    public const string GalleryMember = "GalleryMember";

    public const string GalleryOwner = "GalleryOwner";
}
