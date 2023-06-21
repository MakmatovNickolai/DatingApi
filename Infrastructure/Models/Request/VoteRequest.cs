namespace Infrastructure.Models.Request;

public class VoteRequest
{
    public Guid ProfileId { get; set; }
    public bool Liked { get; set; }
}