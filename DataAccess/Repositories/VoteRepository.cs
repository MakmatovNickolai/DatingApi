using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public interface IVoteRepository : ICompositeKeyRepository<Vote>
{
    Task<List<Guid>> GetReceivedVoteProfileIdsAsync(Guid targetProfileId, bool isLiked);
    Task<List<Guid>> GetGivenVoteProfileIdsAsync(Guid voterId, bool isLiked);
}

public class VoteRepository : CompositeKeyRepository<Vote>, IVoteRepository
{
    private readonly ApplicationDbContext _context;
    public VoteRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Guid>> GetReceivedVoteProfileIdsAsync(Guid receiverId, bool isLiked)
    {
        var profileIds = await GetAll()
            .Where(x => x.IsLiked == isLiked && x.ReceiverId == receiverId)
            .Select(x => x.VoterId)
            .ToListAsync();

        return profileIds;
    }

    public async Task<List<Guid>> GetGivenVoteProfileIdsAsync(Guid voterId, bool isLiked)
    {
        var profileIds = await GetAll()
            .Where(x => x.IsLiked == isLiked && x.VoterId == voterId)
            .Select(x => x.ReceiverId)
            .ToListAsync();

        return profileIds;
    }
}
