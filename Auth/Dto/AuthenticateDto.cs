using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

public class AuthenticateDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}