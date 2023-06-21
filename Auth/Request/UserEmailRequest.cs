using System.ComponentModel.DataAnnotations;

namespace Auth.Request;

public class UserEmailRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
}