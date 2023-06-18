namespace Auth.Config;

public class UserValidation
{
    public int MinPasswordLength { get; set; }
    public int CodeLifetime { get; set; }
    public int VerificationCodeLength { get; set; }
}
