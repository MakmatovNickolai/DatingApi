namespace Infrastructure.Models;

public class VoteDto
{
    public Guid ProfileId { get; set; }
    public bool Liked { get; set; }
}