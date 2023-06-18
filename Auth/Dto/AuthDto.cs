namespace Auth.Dto;

public class AuthDto
{
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public Guid UserProfileId { get; set; }
    public string Name { get; set; }
    public bool IsEmailConfirmed { get; set; }
}