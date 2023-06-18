using Common.Models;

namespace Infrastructure.Models.Users;

public class LikedMeResponse : BaseResponse
{
    public LikedUserListDto Profiles { get; set; }
}
