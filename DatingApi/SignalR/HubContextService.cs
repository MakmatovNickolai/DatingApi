using DatingApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DatingApi.SignalR;

public class HubContextService
{
    private readonly IHubContext<ConversationHub> _hubContext;
    private readonly ILogger<HubContextService> _logger;

    public HubContextService(IHubContext<ConversationHub> hubContext, ILogger<HubContextService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendMessageAsync(string profileId, Guid conversationId, string message)
    {
        _logger.LogInformation($"Sending message to: {profileId}, conversationId: {conversationId}, message: {message}");
        await _hubContext.Clients.User(profileId).SendAsync("ConversationMessage", conversationId, message);
    }

    public async Task MarkMessageAsReadAsync(string profileId, Guid conversationId, int messageId)
    {
        _logger.LogInformation($"MessageRead: MessageSenderId: {profileId}, conversationId: {conversationId}, messageId: {messageId}");
        await _hubContext.Clients.User(profileId).SendAsync("MessageRead", messageId);
    }
}
