namespace Infrastructure.Config;

public class NotificationOptions
{
    public string Server { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public bool Secure { get; set; }
}