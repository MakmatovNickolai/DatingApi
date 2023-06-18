using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

public class UserEmailDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; }
}