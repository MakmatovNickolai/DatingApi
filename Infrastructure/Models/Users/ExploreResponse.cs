using Common.Models;

namespace Infrastructure.Models.Users;

public class ExploreResponse : BaseResponse
{
    public List<ProfileDto> Profiles { get; set; }
}
