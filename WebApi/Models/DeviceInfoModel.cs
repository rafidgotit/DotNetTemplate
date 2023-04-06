namespace Sugary.WebApi.Models;

public class DeviceInfoModel
{
    public string DeviceId { get; set; }
    public string ModelName { get; set; }
    public string BrandName { get; set; }
    public string IpAddress { get; set; }
    public bool IsPhysicalDevice { get; set; } = true;
    public string Platform { get; set; }
    public string SystemVersion { get; set; } // VersionReleaseName for android, systemVersion for ios
    public string SystemId { get; set; }  // VersionSdk for andoroid, Machine for ios
    public string FcmToken { get; set; }
}