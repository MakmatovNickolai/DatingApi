namespace Infrastructure.Models.Dto;

public class UserDto
{
    public Guid Id { get; set; }
    public string CreatedOn { get; set; }
    public string Email { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsGoogleUser { get; set; }
    public bool IsFacebookUser { get; set; }
    public bool IsAppleUser { get; set; }
    public bool IsBlocked { get; set; }
    public string? RefreshToken { get; set; }
    public string AppVersion { get; set; }
    public string Platform { get; set; }
    public string DeviceInfo { get; set; }
    public string DeviceUniqueId { get; set; }
    public string DeviceLocale { get; set; }
    public string DeviceGmtOffset { get; set; }
    public bool IsRootedDevice { get; set; }
    public bool IsEmulator { get; set; }
    public string? GoogleId { get; set; }
    public int FilterDistance { get; set; }
    public int FilterAgeMin { get; set; }
    public int FilterAgeMax { get; set; }
    public ProfileDto Profile { get; set; }
}