using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.Users;

public class UserDetailUpdateRequest
{
    [StringLength(8, MinimumLength = 2, ErrorMessage = "Name length must be between {2} and {1} characters")]
    public string Name { get; set; }
    [MinLength(2)]
    public Guid[] ImageIds { get; set; }
    public string? Password { get; set; }

    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    public string Description { get; set; }

}
