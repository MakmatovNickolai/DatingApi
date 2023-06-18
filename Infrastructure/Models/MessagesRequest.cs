using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models;

public class MessagesRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "The offset must be positive.")]
    public int Offset { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "The limit must be positive.")]
    public int Limit { get; set; }
    public Guid ConversationId { get; set; }
}
