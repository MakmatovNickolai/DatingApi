using Common.Models;

namespace Infrastructure.Models;

public class MessagesResponse : BaseResponse
{
    public List<MessageDto> Messages { get; set; }
}
