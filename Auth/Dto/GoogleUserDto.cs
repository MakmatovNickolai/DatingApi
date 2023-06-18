using System.ComponentModel.DataAnnotations;

namespace Auth.Dto;

public class GoogleUserDto
{
    [Required]
    public string GoogleId { get; set; }
    [Required]
    [EmailAddress]
    public string GoogleEmail { get; set; }
}