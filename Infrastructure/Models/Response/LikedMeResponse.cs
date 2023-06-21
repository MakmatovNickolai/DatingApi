using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class LikedMeResponse : BaseResponse
{
    public LikedUserListDto Profiles { get; set; }
}