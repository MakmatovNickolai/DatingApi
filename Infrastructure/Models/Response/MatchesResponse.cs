using Common.Models;
using Infrastructure.Models.Dto;

namespace Infrastructure.Models.Response;

public class MatchesResponse : BaseResponse
{
    public MatchedUserListDto Matches { get; set; }
}