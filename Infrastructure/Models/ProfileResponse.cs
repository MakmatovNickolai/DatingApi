using Common.Models;
using Infrastructure.Models.Users;

namespace Infrastructure.Models;

public class ProfileResponse : BaseResponse
{
    public ProfileDto Profile { get; set; }

}
