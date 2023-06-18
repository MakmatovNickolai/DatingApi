using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using DataAccess.Entities;
using DataAccess.Repositories;
using Infrastructure.Models;
using Infrastructure.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public interface IUserService
{
    Task DeleteAsync(Guid userId);
    Task<LikedUserListDto> GetLikedProfilesAsync(Guid targetProfileId, PaginationRequest request);
    Task<MatchedUserListDto> GetMatchedProfilesAsync(Guid targetProfileId, PaginationRequest request);
    Task UpdateLocationAsync(Guid profileId, UpdateLocationDto data);
    Task<List<ProfileDto>> GetClosestProfilesAsync(Guid userId, ExploreRequest request);
    Task<bool> VoteAsync(Guid voterId, VoteDto data);
    Task UpdateUserDetailsAsync(Guid userId, UserDetailUpdateRequest data);
    Task<ProfileDto> GetProfileDetailsAsync(Guid profileId);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserValidatorService _userValidator;
    private const int ExploreRequestCount = 10; // TODO: move to config

    public UserService(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IUserValidatorService userValidator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userValidator = userValidator;
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _unitOfWork.UserRepository.DeleteAsync(userId);
    }

    public async Task<List<ProfileDto>> GetClosestProfilesAsync(Guid profileId, ExploreRequest request)
    {
        var latitude = request.Latitude;
        var longitude = request.Longitude;
        var radius = request.Radius;
        List<Guid> profileIds = await _unitOfWork.ProfileRepository.GetDbSet()
            .FromSqlInterpolated(
                $@"SELECT ""Id"" FROM(
                        SELECT ""Id"", 
                        (6371 *
                            acos(cos(radians({latitude})) *
                                cos(radians(""Latitude"")) *
                                cos(radians(""Longitude"") - radians({longitude})) +
                                sin(radians({latitude})) *
                                sin(radians(""Latitude""))
                        )) AS ""Distance"" from ""Profile""
                    ) x
                    WHERE ""Distance"" < {radius}
                    ORDER BY ""Distance""")
            .Select(x => x.Id)
            .ToListAsync();

        profileIds.Remove(profileId);

        var result = await _unitOfWork.ProfileRepository
            .GetAll()
            .Where(x => profileIds.Contains(x.Id) && !x.VotesGiven.Any(vote => vote.VoterId == profileId)) // exclude already voted profiles
            .Take(ExploreRequestCount)
            .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return result;
    }

    public async Task<bool> VoteAsync(Guid voterId, VoteDto data)
    {
        await _unitOfWork.VoteRepository.FailIfAnyAsync(x => x.VoterId == voterId && x.ReceiverId == data.ProfileId);
        var profileExists = await _unitOfWork.ProfileRepository.CountAsync(x => x.Id == data.ProfileId) > 0;
        if (!profileExists)
        {
            throw new BadRequestException("Target profile does not exist");
        }
        await _unitOfWork.VoteRepository.InsertAsync(new Vote()
        {
            VoterId = voterId,
            ReceiverId = data.ProfileId,
            IsLiked = data.Liked
        });
        var isMutualVote = await _unitOfWork.VoteRepository.CountAsync(x => x.VoterId == data.ProfileId && x.ReceiverId == voterId && x.IsLiked) > 0;

        return isMutualVote;
    }

    public async Task<LikedUserListDto> GetLikedProfilesAsync(Guid receiverId, PaginationRequest request)
    {
        var likedCount = await _unitOfWork.VoteRepository.CountAsync(x => x.ReceiverId == receiverId && x.IsLiked);

        var givenLikes = await _unitOfWork.VoteRepository.GetGivenVoteProfileIdsAsync(receiverId, true);

        var likedUsers = await _unitOfWork.VoteRepository
            .GetAll()
            .Where(x => x.ReceiverId == receiverId && x.IsLiked && !givenLikes.Contains(x.VoterId)) // non mutual likes
            .OrderByDescending(x => x.CreatedOn)
            .Skip(request.Offset)
            .Take(request.Limit)
            .ProjectTo<LikeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new LikedUserListDto
        {
            LikedUsers = likedUsers,
            TotalCount = likedCount
        };
    }

    public async Task<MatchedUserListDto> GetMatchedProfilesAsync(Guid targetProfileId, PaginationRequest request)
    {
        // todo wrong matches
        var exisitingParticipantIds = await _unitOfWork.ConversationRepository.GetExistingParticipantIdsAsync(targetProfileId);
        var likedMeProfileIds = await _unitOfWork.VoteRepository.GetReceivedVoteProfileIdsAsync(targetProfileId, true);
        likedMeProfileIds.Where(x => !exisitingParticipantIds.Contains(x));

        var matchedProfiles = await _unitOfWork.ProfileRepository
            .GetAll()
            .Where(profile =>
                likedMeProfileIds.Contains(profile.Id) &&
                profile.VotesReceived.Any(vote => vote.VoterId == targetProfileId)) // I liked
                                                                                    //.OrderByDescending(x => x.CreatedOn) todo how to get matched date?
            .Skip(request.Offset)
            .Take(request.Limit)
            .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new MatchedUserListDto
        {
            MatchedUsers = matchedProfiles,
            TotalCount = matchedProfiles.Count
        };
    }

    public async Task UpdateLocationAsync(Guid profileId, UpdateLocationDto data)
    {
        await _unitOfWork.ProfileRepository.UpdateAsync(profileId, (u, ctx) =>
        {
            u.Entity.Latitude = data.Latitude;
            u.Entity.Longitude = data.Longitude;
        });
    }

    public async Task UpdateUserDetailsAsync(Guid userId, UserDetailUpdateRequest data)
    {
        var user = await _unitOfWork.UserRepository.GetAsync(userId);
        if (user == null)
        {
            throw new BadRequestException("No such user");
        }
        var profile = await _unitOfWork.ProfileRepository.GetAsync(x => x.UserId == userId);
        if (profile == null)
        {
            throw new BadRequestException("No such profile");
        }
        await _userValidator.CheckImagesAsync(data.ImageIds);
        await _unitOfWork.ProfileRepository.UpdateAsync(userId, (u, ctx) =>
        {
            u.Entity.Name = data.Name;
            u.Entity.BirthDate = DateOnly.FromDateTime(data.BirthDate);
            u.Entity.Description = data.Description;
        });
        foreach (var imageId in data.ImageIds)
        {
            await _unitOfWork.ImageRepository.UpdateAsync(imageId, (u, ctx) =>
            {
                u.Entity.ProfileId = profile.Id;
            });
        }
    }

    public async Task<ProfileDto> GetProfileDetailsAsync(Guid profileId)
    {
        var profile = await _unitOfWork.ProfileRepository
            .GetAll(x => x.Gender, x => x.Images).FirstOrDefaultAsync(x => x.Id == profileId);
        if (profile == null)
        {
            throw new BadRequestException("No such profile");
        }
        return _mapper.Map<ProfileDto>(profile);
    }
}
