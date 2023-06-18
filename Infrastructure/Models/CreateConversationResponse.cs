using Common.Models;

namespace Infrastructure.Models;

public class CreateConversationResponse : BaseResponse
{
    public Guid ConversationId { get; set; }
}
