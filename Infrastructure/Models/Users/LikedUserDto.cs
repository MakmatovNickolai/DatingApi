namespace Infrastructure.Models.Users;

public class LikedUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string LikeDate { get; set; }
    public Guid[] ImageIds { get; set; }
}