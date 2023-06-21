namespace Auth.Request;

public class RegisterGoogleRequest : RegisterRequest
{
    public string GoogleId { get; set; }
    public string GoogleToken { get; set; }
}