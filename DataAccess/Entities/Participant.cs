using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Participant
{
    public Guid ConversationId { get; set; }
    public Guid ProfileId { get; set; }
    public Profile Profile { get; set; }
    public Conversation Conversation { get; set; }
}

public static class ParticipantConfiguration
{
    public static void ConfigureParticipants(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participant>().HasKey(x => new { x.ConversationId, x.ProfileId });

        modelBuilder.Entity<Participant>()
            .HasOne(x => x.Conversation)
            .WithMany(x => x.Participants);
    }
}