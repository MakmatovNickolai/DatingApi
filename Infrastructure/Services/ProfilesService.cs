using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using DataAccess.Entities;
using DataAccess.Repositories;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public interface IProfilesService
{
    Task<bool> VoteAsync(Guid voterId, VoteRequest data);
    Task<LikedUserListDto> GetLikedAsync(Guid targetProfileId, PaginationRequest request);
    Task<MatchedUserListDto> GetMatchedAsync(Guid targetProfileId, PaginationRequest request);
    Task<List<ProfileDto>> GetClosestAsync(Guid userId, ExploreRequest request);
}

public class ProfilesService : IProfilesService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int ExploreRequestCount = 10; // TODO: move to config

    public ProfilesService(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> VoteAsync(Guid voterId, VoteRequest data)
    {
        await _unitOfWork.VoteRepository.FailIfAnyAsync(x => x.VoterId == voterId && x.ReceiverId == data.ProfileId);
        var profileExists = await _unitOfWork.ProfileRepository.CountAsync(x => x.Id == data.ProfileId) > 0;
        if (!profileExists)
        {
            throw new BadRequestException("Target profile does not exist");
        }
        var givenVote = await _unitOfWork.VoteRepository.InsertAsync(new Vote()
        {
            VoterId = voterId,
            ReceiverId = data.ProfileId,
            IsLiked = data.Liked
        });
        var isMutualVote = await _unitOfWork.VoteRepository
            .CountAsync(x => x.VoterId == data.ProfileId && x.ReceiverId == voterId && x.IsLiked) > 0;
        if (isMutualVote)
        {
            var matchDate = DateTime.UtcNow;
            await _unitOfWork.VoteRepository.UpdateAsync((u, ctx) =>
            {
                u.Entity.IsMatched = true;
                u.Entity.MatchedOn = matchDate;
            }, givenVote.VoterId, givenVote.ReceiverId);

            var receivedVote = await _unitOfWork
                .VoteRepository
                .GetAsync(data.ProfileId, voterId);

            await _unitOfWork.VoteRepository.UpdateAsync((u, ctx) =>
            {
                u.Entity.IsMatched = true;
                u.Entity.MatchedOn = matchDate;
            }, receivedVote!.VoterId, receivedVote.ReceiverId);
        }

        return isMutualVote;
    }

    public async Task<List<ProfileDto>> GetClosestAsync(Guid profileId, ExploreRequest request)
    {
        var latitude = request.Latitude;
        var longitude = request.Longitude;
        var radius = request.Radius;
        var profiles = await _unitOfWork.ProfileRepository.GetClosestAsync(profileId, latitude, longitude, radius, ExploreRequestCount);
        var newProfiles = profiles.Select(p => new ProfileDto
        {
            Id = p.Id,
            DateOfBirth = p.DateOfBirth,
            Description = p.Description,
            DistanceInMeters = p.DistanceInMeters,
            Gender = p.Gender,
            ImageIds = profiles.Where(pr => pr.Id == p.Id).Select(pr => pr.ImageId).ToArray(),
            Name = p.Name,
            RecentOn = p.RecentOn,
            UserId = p.UserId
        }).DistinctBy(p => p.Id).ToList();

        return newProfiles;
    }

    public async Task<LikedUserListDto> GetLikedAsync(Guid receiverId, PaginationRequest request)
    {
        var likedCount = await _unitOfWork.VoteRepository.CountAsync(x => x.ReceiverId == receiverId && x.IsLiked && !x.IsMatched);

        var likedUsers = await _unitOfWork.VoteRepository
            .GetAll()
            .OrderByDescending(x => x.CreatedOn)
            .Where(vote =>
                vote.ReceiverId == receiverId &&
                !vote.Voter.User!.IsBlocked &&
                vote.IsLiked &&
                !vote.IsMatched) // non mutual likes
            .Skip(request.Offset) // todo rewrite to keyset pagination
            .Take(request.Limit)
            .ProjectTo<LikeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new LikedUserListDto
        {
            LikedUsers = likedUsers,
            TotalCount = likedCount
        };
    }

    public async Task<MatchedUserListDto> GetMatchedAsync(Guid receiverId, PaginationRequest request)
    {
        var exisitingParticipantIds = await _unitOfWork.ConversationRepository.GetExistingParticipantIdsAsync(receiverId);

        var totalCount = await _unitOfWork.VoteRepository
            .CountAsync(x => x.ReceiverId == receiverId && x.IsMatched && !x.Voter.User!.IsBlocked && !exisitingParticipantIds.Contains(x.VoterId));

        var matches = await _unitOfWork.VoteRepository
            .GetAll()
            .Where(vote =>
                vote.ReceiverId == receiverId &&
                vote.IsMatched &&
                !vote.Voter.User!.IsBlocked &&
                !exisitingParticipantIds.Contains(vote.VoterId))
            .Skip(request.Offset)
            .Take(request.Limit)
            .ProjectTo<MatchDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new MatchedUserListDto
        {
            Profiles = matches,
            TotalCount = totalCount
        };
    }
}