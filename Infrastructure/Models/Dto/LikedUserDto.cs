namespace Infrastructure.Models.Dto;

public class LikedUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string LikedOn { get; set; }
    public Guid[] ImageIds { get; set; }
}