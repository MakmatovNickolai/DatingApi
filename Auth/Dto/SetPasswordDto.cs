namespace Auth.Dto;

public class SetPasswordDto
{
    public Guid UserId { get; set; }
    public string Password { get; set; }
}
