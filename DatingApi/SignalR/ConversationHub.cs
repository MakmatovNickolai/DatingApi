using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApi.SignalR;

[Authorize]
public class ConversationHub : Hub
{
    private readonly ILogger<ConversationHub> _logger;
    public ConversationHub(ILogger<ConversationHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"User connected profileId: {Context.UserIdentifier}");

        return base.OnConnectedAsync();
    }

    public async Task Typing(string typingByProfileId, Guid conversationId, bool isTyping)
    {
        _logger.LogInformation($"Typing: {typingByProfileId}, {conversationId}, {isTyping}");
        await Clients.User(typingByProfileId).SendAsync("Typing", isTyping);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"User disconnected: {Context.User.Identity?.Name}, {Context.UserIdentifier}");
        return base.OnDisconnectedAsync(exception);
    }
}