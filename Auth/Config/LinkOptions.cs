namespace Auth.Config;

public class LinkOptions
{
    public string FrontHost { get; set; }
    public string BackendHost { get; set; }
    public string NewPasswordRoute { get; set; }
    public string ConfirmEmailRoute { get; set; }

    public string GetConfirmUrl(Guid id, string code)
    {
        return $"{BackendHost}/api/auth/confirm?userId={id}&code={code}";
    }

    public string GetRestorePasswordUrl(Guid id, string code)
    {
        return $"{FrontHost}/auth/restore?userId={id}&code={code}";
    }

    public string ConfirmEmailRedirectUrl { get => FrontHost + ConfirmEmailRoute; }
    public string NewPasswordRedirectUrl { get => FrontHost + NewPasswordRoute; }
}
