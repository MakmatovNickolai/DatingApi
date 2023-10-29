using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using DataAccess.Entities;
using DataAccess.Models;
using DataAccess.Repositories;
using Infrastructure.Helpers.Geolocation;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public interface IProfileService
{
    Task<bool> VoteAsync(Guid voterId, VoteRequest data);
    Task<LikedUserListDto> GetLikedAsync(Guid targetProfileId, PaginationRequest request);
    Task<MatchedUserListDto> GetMatchedAsync(Guid targetProfileId, PaginationRequest request);
    Task<List<ProfileDto>> GetClosestAsync(Guid userId, ExploreRequest request);
    Task<ProfileDto> GetDetailsAsync(Guid currentProfileId, Guid id);
}

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int ExploreRequestCount = 10 * 5; // TODO: move to config, 5 is for 5 images

    public ProfileService(
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> VoteAsync(Guid voterId, VoteRequest data)
    {       
        await _unitOfWork.VoteRepository.FailIfAnyAsync(x => x.VoterId == voterId && x.ReceiverId == data.ProfileId); // todo why duplicate object exception throws occasionally here?
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
        var searchOptions = await _unitOfWork.ProfileRepository.GetAll().Where(x => x.Id == profileId).Select(p => new SearchOptions
        {
            Distance = p.User!.FilterDistance,
            AgeMax = p.User!.FilterAgeMax,
            AgeMin = p.User!.FilterAgeMin
        }).FirstAsync();

        searchOptions.Latitude = request.Latitude;
        searchOptions.Longitude = request.Longitude;
        searchOptions.ProfileId = profileId;
        searchOptions.Limit = ExploreRequestCount;

        var profileDbDtos = await _unitOfWork.ProfileRepository.GetClosestAsync(searchOptions);
        
        var profiles = profileDbDtos.Select(p => new ProfileDto
        {
            Id = p.Id,
            DateOfBirth = p.DateOfBirth,
            Description = p.Description,
            DistanceKm = p.DistanceKm,
            Gender = p.Gender,
            Images = profileDbDtos
                .Where(pr => pr.Id == p.Id)
                .Select(pr => new ImageDto {
                    Id = pr.ImageId,
                    OrderIndex = pr.OrderIndex,
                }).ToArray(),
            Name = p.Name,
            RecentOn = p.RecentOn,
            UserId = p.UserId
        }).DistinctBy(p => p.Id).ToList();

        return profiles;
    }

    public async Task<LikedUserListDto> GetLikedAsync(Guid receiverId, PaginationRequest request)
    {
        var currentProfile = await _unitOfWork.ProfileRepository.GetAsync(receiverId);

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
            .ProjectTo<LikeDto>(_mapper.ConfigurationProvider, new
            {
                currentProfileCoordinate = new GeoCoordinate(currentProfile.Latitude, currentProfile.Longitude)
            })
            .ToListAsync();

        return new LikedUserListDto
        {
            LikedUsers = likedUsers,
            TotalCount = likedCount
        };
    }

    public async Task<MatchedUserListDto> GetMatchedAsync(Guid receiverId, PaginationRequest request)
    {
        var currentProfile = await _unitOfWork.ProfileRepository.GetAsync(receiverId);

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
            .ProjectTo<MatchDto>(_mapper.ConfigurationProvider, new
            {
                currentProfileCoordinate = new GeoCoordinate(currentProfile.Latitude, currentProfile.Longitude)
            })
            .ToListAsync();

        return new MatchedUserListDto
        {
            Profiles = matches,
            TotalCount = totalCount
        };
    }

    public async Task<ProfileDto> GetDetailsAsync(Guid currentProfileId, Guid id)
    {
        var profile = await _unitOfWork.ProfileRepository.GetAsync(id);
        if (profile == null)
        {
            throw new BadRequestException("Such profile does not exist");
        }

        var currentProfile = await _unitOfWork.ProfileRepository.GetAsync(currentProfileId);
        
        var distance = DistanceCalculator.CalculateDistance(
            new GeoCoordinate(currentProfile.Latitude, currentProfile.Longitude),
            new GeoCoordinate(profile.Latitude, profile.Longitude)
        );
        var profileDto = _mapper.Map<ProfileDto>(profile);
        profileDto.DistanceKm = Convert.ToInt32(distance);
        return profileDto;
    }
}