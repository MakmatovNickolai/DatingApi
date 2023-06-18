namespace Infrastructure.Models.Users;

public class MatchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid[] ImageIds { get; set; }
}