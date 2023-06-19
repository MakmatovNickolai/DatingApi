using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Vote : ITrackableEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public Guid VoterId { get; set; }
    public Guid ReceiverId { get; set; }
    public bool IsLiked { get; set; }
    public bool IsMatched { get; set; }
    public DateTime MatchedOn { get; set; }
    public virtual Profile Voter { get; set; }
    public virtual Profile Receiver { get; set; }
}

public static class VoteConfiguration
{
    public static void ConfigureVotes(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vote>().HasKey(x => new { x.VoterId, x.ReceiverId });

        modelBuilder.Entity<Vote>()
            .HasOne(sc => sc.Voter)
            .WithMany(s => s.VotesGiven);

        modelBuilder.Entity<Vote>()
            .HasOne(sc => sc.Receiver)
            .WithMany(s => s.VotesReceived);
    }
}