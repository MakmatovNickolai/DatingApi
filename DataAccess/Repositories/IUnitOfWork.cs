using DataAccess.Entities;
using DataAccess.Entities.Identity;

namespace DataAccess.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task SaveChangesAsync();
    void SaveChanges();
    IRepository<User, Guid> UserRepository { get; }
    IRepository<Profile, Guid> ProfileRepository { get; }
    IRepository<Image, Guid> ImageRepository { get; }
    IRepository<Gender, Guid> GenderRepository { get; }
    IRepository<VerificationCode, Guid> VerificationCodeRepository { get; }
    IConversationRepository ConversationRepository { get; }
    IVoteRepository VoteRepository { get; }
    ICompositeKeyRepository<Participant> ParticipantRepository { get; }
    IRepository<Message, int> MessageRepository { get; }

}
