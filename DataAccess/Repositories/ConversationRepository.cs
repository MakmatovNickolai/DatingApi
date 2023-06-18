using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public interface IConversationRepository : IRepository<Conversation, Guid>
{
    Task<List<Guid>> GetExistingParticipantIdsAsync(Guid targetProfileId);
}


public class ConversationRepository : Repository<Conversation, Guid>, IConversationRepository
{
    private readonly ApplicationDbContext _context;
    public ConversationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<Guid>> GetExistingParticipantIdsAsync(Guid targetProfileId)
    {
        var exisitingParticipantIds = await GetAll()
            .Where(x => x.Participants.Any(p => p.ProfileId == targetProfileId))
            .Select(x => x.Participants.FirstOrDefault(p => p.ProfileId != targetProfileId)!.ProfileId)
            .ToListAsync();
        return exisitingParticipantIds;
    }
}
