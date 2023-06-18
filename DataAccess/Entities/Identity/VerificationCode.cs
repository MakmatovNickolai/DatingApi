using DataAccess.Entities.Base;

namespace DataAccess.Entities.Identity;

public enum VerificationCodeType
{
    EmailConfirm = 1,
    RestorePassword = 2
}

public class VerificationCode : Entity<Guid>
{
    public VerificationCodeType Type { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}
