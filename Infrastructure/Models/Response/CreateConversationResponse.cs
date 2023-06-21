using Common.Models;

namespace Infrastructure.Models.Response;

public class CreateConversationResponse : BaseResponse
{
    public Guid ConversationId { get; set; }
}