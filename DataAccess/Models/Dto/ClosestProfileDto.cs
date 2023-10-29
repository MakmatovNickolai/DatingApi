namespace DataAccess.Models.Dto;

public class ClosestProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DateOfBirth { get; set; }
    public string Description { get; set; }
    public string? RecentOn { get; set; }
    public string Gender { get; set; }
    public Guid ImageId { get; set; }
    public int OrderIndex { get; set; }
    public Guid? UserId { get; set; }
    public int DistanceKm { get; set; }
}
