namespace Infrastructure.Models.Dto;

public class MatchedUserListDto
{
    public List<MatchDto> Profiles { get; set; }
    public int TotalCount { get; set; }
}