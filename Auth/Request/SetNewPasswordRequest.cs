namespace Auth.Request;

public class SetNewPasswordRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}
