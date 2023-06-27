namespace Infrastructure.Models.Request;

public class MarkAsReadRequest
{
    public Guid ConversationId { get; set; }
    public int MessageId { get; set; }
}