using Common.Models;

namespace Infrastructure.Models.Response;

public class SendMessageResponse : BaseResponse
{
    public int MessageId { get; set; }
}