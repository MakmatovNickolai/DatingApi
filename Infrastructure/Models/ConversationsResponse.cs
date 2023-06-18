using Common.Models;

namespace Infrastructure.Models;

public class ConversationsResponse : BaseResponse
{
    public List<ConversationDto> Conversations { get; set; }
}
