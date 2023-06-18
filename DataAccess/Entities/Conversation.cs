using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Conversation : Entity<Guid>, ITrackableEntity
{
    public virtual ICollection<Message> Messages { get; set; }
    public virtual ICollection<Participant> Participants { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}

public static class ConversationConfiguration
{
    public static void ConfigureConversations(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>()
                  .HasMany(x => x.Participants)
                  .WithOne(x => x.Conversation);

        modelBuilder.Entity<Conversation>()
                        .HasMany(x => x.Messages)
                        .WithOne(x => x.Conversation);
    }
}
