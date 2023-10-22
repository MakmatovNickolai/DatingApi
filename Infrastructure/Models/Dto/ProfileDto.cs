namespace Infrastructure.Models.Dto;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Gender { get; set; }
    public string Description { get; set; }
    public string DateOfBirth { get; set; }
    public string RecentOn { get; set; }
    public int DistanceInMeters { get; set; }
    public Guid? UserId { get; set; }
    public Guid[] ImageIds { get; set; }
}