namespace Infrastructure.Models.Dto;

public class UserDetailDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageId { get; set; }
}