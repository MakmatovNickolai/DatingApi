namespace Auth.Dto;

public class ConfirmUserDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
}