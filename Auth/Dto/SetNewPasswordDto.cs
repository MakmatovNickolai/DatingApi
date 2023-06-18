namespace Auth.Dto;

public class SetNewPasswordDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}
