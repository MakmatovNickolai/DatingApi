namespace Infrastructure.Models.Users;

public class MatchedUserListDto
{
    public List<MatchDto> Profiles { get; set; }
    public int TotalCount { get; set; }
}