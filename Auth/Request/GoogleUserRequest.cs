using System.ComponentModel.DataAnnotations;

namespace Auth.Request;

public class GoogleUserRequest
{
    [Required]
    public string GoogleId { get; set; }
    [Required]
    [EmailAddress]
    public string GoogleEmail { get; set; }
}