using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class ConversationsResponse : BaseResponse
{
    public List<ConversationDto> Conversations { get; set; }
}