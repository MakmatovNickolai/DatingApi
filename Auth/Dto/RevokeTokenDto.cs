using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

public class RevokeTokenDto
{
    [Required]
    public string RefreshToken { get; set; }
}