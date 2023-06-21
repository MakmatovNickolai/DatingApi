using System.ComponentModel.DataAnnotations;

namespace Auth.Request;

public class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; }
}