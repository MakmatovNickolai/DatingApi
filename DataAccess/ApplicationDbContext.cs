using DataAccess.Entities;
using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContext)
        : base(dbContext)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureUsers();
        modelBuilder.ConfigureProfiles();
        modelBuilder.ConfigureVotes();
        modelBuilder.ConfigureConversations();
        modelBuilder.ConfigureMessages();
        modelBuilder.ConfigureParticipants();
        modelBuilder.ConfigureGenders();
    }

    public override int SaveChanges()
    {
        SetProperties();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetProperties()
    {
        var entries = ChangeTracker
        .Entries()
        .Where(e => e.Entity is ITrackableEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((ITrackableEntity)entityEntry.Entity).ModifiedOn = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((ITrackableEntity)entityEntry.Entity).CreatedOn = DateTime.UtcNow;
            }
        }
    }
}
