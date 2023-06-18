namespace Infrastructure.Models.Users;

public class RoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public string TimeAgo { get; set; }
    public Guid ImageId { get; set; }
}