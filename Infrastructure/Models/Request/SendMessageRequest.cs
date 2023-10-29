namespace Infrastructure.Models.Request;

public class SendMessageRequest
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; }
}