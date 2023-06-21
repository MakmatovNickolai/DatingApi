namespace Infrastructure.Models.Dto;

public class LikedUserListDto
{
    public List<LikeDto> LikedUsers { get; set; }
    public int TotalCount { get; set; }
}