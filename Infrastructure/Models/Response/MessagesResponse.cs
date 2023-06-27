using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class MessagesResponse : BaseResponse
{
    public MessagesDto Response { get; set; }
}