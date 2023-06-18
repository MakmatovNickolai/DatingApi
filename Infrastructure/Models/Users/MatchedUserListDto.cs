namespace Infrastructure.Models.Users;

public class MatchedUserListDto
{
    public List<ProfileDto> MatchedUsers { get; set; }
    public int TotalCount { get; set; }
}