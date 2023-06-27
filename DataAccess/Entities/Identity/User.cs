using DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Entities.Identity;

public class User : Entity<Guid>, ITrackableEntity
{
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public string Email { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsGoogleUser { get; set; }
    public bool IsFacebookUser { get; set; }
    public bool IsAppleUser { get; set; }
    public bool IsBlocked { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string AppVersion { get; set; }
    public string Platform { get; set; }
    public string DeviceInfo { get; set; }
    public string DeviceUniqueId { get; set; }
    public string DeviceLocale { get; set; }
    public string DeviceGmtOffset { get; set; }
    public bool IsRootedDevice { get; set; }
    public bool IsEmulator { get; set; }
    public string? GoogleId { get; set; }
    public int FilterDistance { get; set; } = 10;
    public int FilterAgeMin { get; set; } = 18;
    public int FilterAgeMax { get; set; } = 100;
    public virtual Profile Profile { get; set; }
    public virtual ICollection<VerificationCode> VerificationCodes { get; set; }
}

public static class UserConfiguration
{
    public static void ConfigureUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(x => x.VerificationCodes)
            .WithOne(x => x.User);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User);
    }
}