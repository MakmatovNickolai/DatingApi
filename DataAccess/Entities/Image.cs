using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities;

public class Image : Entity<Guid>, ITrackableEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string ContentType { get; set; }
    public byte[] FileData { get; set; }
    public Guid? ProfileId { get; set; }
    public bool IsTemporary { get; set; }
    public int OrderIndex { get; set; }
    public virtual Profile? Profile { get; set; }
}

public static class ImageConfiguration
{
    public static void ConfigureImages(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Image>()
            .HasOne(u => u.Profile)
            .WithMany(u => u.Images);

    }
}