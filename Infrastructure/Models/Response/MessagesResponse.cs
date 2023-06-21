using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class MessagesResponse : BaseResponse
{
    public List<MessageDto> Messages { get; set; }
}