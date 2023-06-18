using DataAccess.Entities;
using DataAccess.Entities.Identity;

namespace DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IVoteRepository _voteRepository;
    public IVoteRepository VoteRepository
    {
        get
        {
            if (_voteRepository == null)
            {
                _voteRepository = new VoteRepository(_context);
            }
            return _voteRepository;
        }
    }

    private ICompositeKeyRepository<Participant> _participantRepository;
    public ICompositeKeyRepository<Participant> ParticipantRepository
    {
        get
        {
            if (_participantRepository == null)
            {
                _participantRepository = new CompositeKeyRepository<Participant>(_context);
            }
            return _participantRepository;
        }
    }

    private ICompositeKeyRepository<Message> _messageRepository;
    public ICompositeKeyRepository<Message> MessageRepository
    {
        get
        {
            if (_messageRepository == null)
            {
                _messageRepository = new CompositeKeyRepository<Message>(_context);
            }
            return _messageRepository;
        }
    }

    private IRepository<User, Guid> _userRepository;
    public IRepository<User, Guid> UserRepository
    {
        get
        {
            if (_userRepository == null)
            {
                _userRepository = new Repository<User, Guid>(_context);
            }
            return _userRepository;
        }
    }

    private IRepository<Profile, Guid> _profileRepository;
    public IRepository<Profile, Guid> ProfileRepository
    {
        get
        {
            if (_profileRepository == null)
            {
                _profileRepository = new Repository<Profile, Guid>(_context);
            }
            return _profileRepository;
        }
    }

    private IRepository<VerificationCode, Guid> _verificationCodeRepository;
    public IRepository<VerificationCode, Guid> VerificationCodeRepository
    {
        get
        {
            if (_verificationCodeRepository == null)
            {
                _verificationCodeRepository = new Repository<VerificationCode, Guid>(_context);
            }
            return _verificationCodeRepository;
        }
    }


    private IConversationRepository _conversationRepository;
    public IConversationRepository ConversationRepository
    {
        get
        {
            if (_conversationRepository == null)
            {
                _conversationRepository = new ConversationRepository(_context);
            }
            return _conversationRepository;
        }
    }

    private IRepository<Gender, Guid> _genderRepository;
    public IRepository<Gender, Guid> GenderRepository
    {
        get
        {
            if (_genderRepository == null)
            {
                _genderRepository = new Repository<Gender, Guid>(_context);
            }
            return _genderRepository;
        }
    }

    private IRepository<Image, Guid> _imageRepository;
    public IRepository<Image, Guid> ImageRepository
    {
        get
        {
            if (_imageRepository == null)
            {
                _imageRepository = new Repository<Image, Guid>(_context);
            }
            return _imageRepository;
        }
    }
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
