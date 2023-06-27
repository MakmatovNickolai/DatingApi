using DataAccess.Entities.Base;
using DataAccess.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Profile : Entity<Guid>, ITrackableEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string Name { get; set; }
    public Guid GenderId { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Description { get; set; }
    public double Longitude { get; set; } = 0;
    public double Latitude { get; set; } = 0;
    public DateTime RecentAt { get; set; } = DateTime.UtcNow;
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    public virtual Gender Gender { get; set; }
    public virtual ICollection<Image> Images { get; set; }
    public virtual ICollection<Vote> VotesReceived { get; set; }
    public virtual ICollection<Vote> VotesGiven { get; set; }
}

public static class ProfileConfiguration
{
    public static void ConfigureProfiles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>()
            .HasOne(u => u.Gender)
            .WithMany(g => g.Profiles);

        modelBuilder.Entity<Profile>()
            .HasMany(x => x.Images)
            .WithOne(x => x.Profile);

        modelBuilder.Entity<Profile>()
           .HasMany(x => x.VotesReceived)
           .WithOne(x => x.Receiver)
           .HasForeignKey(x => x.ReceiverId);

        modelBuilder.Entity<Profile>()
           .HasMany(x => x.VotesGiven)
           .WithOne(x => x.Voter)
           .HasForeignKey(x => x.VoterId);
    }
}