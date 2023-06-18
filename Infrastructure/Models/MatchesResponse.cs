using Common.Models;
using Infrastructure.Models.Users;

namespace Infrastructure.Models;

public class MatchesResponse : BaseResponse
{
    public MatchedUserListDto Matches { get; set; }
}
