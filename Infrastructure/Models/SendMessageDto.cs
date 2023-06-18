namespace Infrastructure.Models;

public class SendMessageDto
{
    public Guid ConversationId { get; set; }
    public string Text { get; set; }
}
