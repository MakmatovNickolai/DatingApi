using Infrastructure.Models.Users;

namespace Infrastructure.Models;

public class LikeDto
{
    public ProfileDto Profile { get; set; }
    public string LikeDate { get; set; }
}
