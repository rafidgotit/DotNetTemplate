namespace DataAccess.Entities;

public class UserClaim
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ReclaimedAt { get; set; }
    public string? DeviceId { get; set; }
    public string? BrandName { get; set; }
    public string? ModelName { get; set; }
    public string? Platform { get; set; }
    public string? SystemVersion { get; set; } // VersionReleaseName for android, systemVersion for ios
    public string? SystemId { get; set; } // VersionSdk for andoroid, Machine for ios
    public DateTime ExpiryAt { get; set; }
    public string? FcmToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? IpAddress { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}