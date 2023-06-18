using Microsoft.AspNetCore.SignalR;

namespace DatingApi.Hubs;

public class ConversationHub : Hub
{
    private readonly ILogger<ConversationHub> _logger;
    public ConversationHub(ILogger<ConversationHub> logger)
    {
        _logger = logger;
    }
    public async Task SendMessage(string user, string message)
    {
        _logger.LogInformation($"Incoming message to: {user}, test: {message}");

        await Clients.Caller.SendAsync("ReceiveNotification", user, message);
        //await Clients.All.SendAsync("ReceiveNotification", user, message);
    }
}
