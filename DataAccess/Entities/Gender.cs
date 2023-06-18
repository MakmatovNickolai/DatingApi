using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Gender : Entity<Guid>
{
    public string Name { get; set; }

    public virtual ICollection<Profile> Profiles { get; set; }
}

public static class GenderConfiguration
{
    public static void ConfigureGenders(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gender>()
            .HasMany(x => x.Profiles)
            .WithOne(x => x.Gender);
    }
}