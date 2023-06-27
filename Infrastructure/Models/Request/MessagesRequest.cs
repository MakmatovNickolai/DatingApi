using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.Request;

public class MessagesRequest
{
    public int LastMessageId { get; set; }
    public string LastMessageCreatedOn { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "The limit must be positive.")]
    public int Limit { get; set; }
    public Guid ConversationId { get; set; }
}