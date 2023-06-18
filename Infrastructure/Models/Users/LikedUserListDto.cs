namespace Infrastructure.Models.Users;

public class LikedUserListDto
{
    public List<LikeDto> LikedUsers { get; set; }
    public int TotalCount { get; set; }
}