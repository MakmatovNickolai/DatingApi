using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

public class TokenDto
{
    [Required]
    public string? AccessToken { get; set; } = null!;
    [Required]
    public string? RefreshToken { get; set; } = null!;
}