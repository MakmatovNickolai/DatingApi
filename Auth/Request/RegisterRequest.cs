using System.ComponentModel.DataAnnotations;

namespace Auth.Request;

public abstract class RegisterRequest
{
    [EmailAddress]
    public string Email { get; set; }

    [StringLength(8, MinimumLength = 2, ErrorMessage = "Name length must be between {2} and {1} characters")]
    public string Name { get; set; }

    public string Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    [MinLength(2)]
    public Guid[] ImageIds { get; set; }
    public string Description { get; set; } = string.Empty;
    public string DeviceInfo { get; set; }
    public string AppVersion { get; set; }
    public string DeviceUniqueId { get; set; }
    public string DeviceLocale { get; set; }
    public bool IsDeviceRooted { get; set; }
    public bool IsDeviceEmulator { get; set; }
    public string DeviceGmtOffset { get; set; }
    public string Platform { get; set; }
}