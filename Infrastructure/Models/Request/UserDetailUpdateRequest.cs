using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.Request;

public class UserDetailUpdateRequest
{
    [StringLength(8, MinimumLength = 2, ErrorMessage = "Name length must be between {2} and {1} characters")]
    public string Name { get; set; }
    [MinLength(2, ErrorMessage = "Minimum 2 images required")]
    [MaxLength(5, ErrorMessage = "Maximum 5 images allowed")]
    public Guid[] ImageIds { get; set; }
    public string? Password { get; set; }

    public string DateOfBirth { get; set; }

    public string Description { get; set; }
}