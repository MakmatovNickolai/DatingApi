using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Message : Entity<int>, ITrackableEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string Text { get; set; }
    public Guid SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public bool IsRead { get; set; }
    public Profile Sender { get; set; }
    public Conversation Conversation { get; set; }
}

public static class MessageConfiguration
{
    public static void ConfigureMessages(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
                .HasOne(x => x.Conversation)
                .WithMany(x => x.Messages);
    }
}
