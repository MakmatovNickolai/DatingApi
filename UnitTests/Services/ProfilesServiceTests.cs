using AutoMapper;
using Common.Exceptions;
using DataAccess.Entities;
using Infrastructure.Mappings;
using Infrastructure.Models.Request;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Services;

public class ProfilesServiceTests
{
    private readonly IMapper _mapper;

    public ProfilesServiceTests()
    {
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DtoMappings());
        });
        _mapper = mockMapper.CreateMapper();
    }

    [Fact]
    public async Task VoteAsync_CreatesARecordInDatabase()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            ProfileId = profile2!.Id,
            Liked = true
        });

        // Assert
        var voteExists = await unitOfWork.VoteRepository.GetAll().CountAsync() > 0;
        Assert.True(voteExists);
    }

    [Fact]
    public async Task VoteAsync_WhenMutualLike_ReturnsTrue()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        var firstLikedSecond = await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        var secondLikedFirst = await profilesService.VoteAsync(profile2.Id, new VoteRequest
        {
            ProfileId = profile1.Id,
            Liked = true
        });

        // Assert
        Assert.True(secondLikedFirst);
    }

    [Fact]
    public async Task VoteAsync_WhenNonMutualLike_ReturnsFalse()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        var firstLikedSecond = await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            ProfileId = profile2!.Id,
            Liked = true
        });

        // Assert
        Assert.False(firstLikedSecond);
    }

    [Fact]
    public async Task VoteAsync_ThrowsBadRequestException_WhenReceiverIdDoesntExist()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await profilesService.VoteAsync(profile1!.Id, new VoteRequest
            {
                ProfileId = Guid.Empty,
                Liked = true
            });
        });
    }

    [Fact]
    public async Task VoteAsync_ThrowsDuplicatedObjectException_WhenVotingAlreadyVotedTarget()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act & Assert
        await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        await Assert.ThrowsAsync<DuplicatedObjectException>(async () =>
        {
            await profilesService.VoteAsync(profile1.Id, new VoteRequest
            {
                ProfileId = profile2.Id,
                Liked = false
            });
        });
    }

    [Fact]
    public async Task VoteAsync_SetsIsMatchedColumn_WhenMutualLike()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        await profilesService.VoteAsync(profile2.Id, new VoteRequest
        {
            ProfileId = profile1.Id,
            Liked = true
        });

        var vote = await unitOfWork.VoteRepository.GetAsync(profile1.Id, profile2.Id);

        // Assert
        Assert.True(vote!.IsMatched);
    }

    [Fact]
    public async Task VoteAsync_DoesntSetIsMatchedColumn_WhenNonMutualLike()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            ProfileId = profile2!.Id,
            Liked = true
        });

        var vote = await unitOfWork.VoteRepository.GetAsync(profile1.Id, profile2.Id);

        // Assert
        Assert.False(vote!.IsMatched);
    }

    [Fact]
    public async Task GetLikedAsync_ReturnsNonMutualLikes()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);
        var profile3 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id && x.Id != profile2!.Id);

        var firstLikedSecond = await profilesService.VoteAsync(profile1.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile2!.Id
        });
        var secondLikedFirst = await profilesService.VoteAsync(profile2.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile1!.Id
        });
        var thirdLikedFirst = await profilesService.VoteAsync(profile3!.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile1!.Id
        });

        // Act
        var result = await profilesService.GetLikedAsync(profile1.Id, new PaginationRequest
        {
            Offset = 0,
            Limit = 10,
        });

        // Assert
        Assert.Single(result.LikedUsers);
    }

    [Fact]
    public async Task GetLikedAsync_DoesntReturnBlockedProfile()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAll().Skip(1).FirstAsync();
        await unitOfWork.UserRepository.UpdateAsync((Guid)profile2.UserId!, (u, ctx) =>
        {
            u.Entity.IsBlocked = true;
        });

        // Act
        await profilesService.VoteAsync(profile2!.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile1!.Id
        });
        var result = await profilesService.GetLikedAsync(profile1.Id, new PaginationRequest
        {
            Offset = 0,
            Limit = 10,
        });

        // Assert
        Assert.DoesNotContain(profile2.Id, result.LikedUsers.Select(x => x.Profile.Id));
    }

    [Fact]
    public async Task GetMatchedAsync_ReturnsOnlyMatchedProfiles()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAll().Skip(1).FirstAsync();
        var profile3 = await unitOfWork.ProfileRepository.GetAll().Skip(2).FirstAsync();
        await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile2!.Id
        });
        await profilesService.VoteAsync(profile2.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile1.Id
        });
        await profilesService.VoteAsync(profile3.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile1.Id
        });

        // Act
        var result = await profilesService.GetMatchedAsync(profile1.Id, new PaginationRequest
        {
            Offset = 0,
            Limit = 10,
        });
        var resultIds = result.Profiles.Select(x => x.Profile.Id);

        // Assert
        Assert.Contains(profile2.Id, resultIds);
        Assert.DoesNotContain(profile3.Id, resultIds);
    }

    [Fact]
    public async Task GetMatched_DoesntReturnProfile_WhenConversationIsAlreadyCreated()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profilesService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAll().Skip(1).FirstAsync();
        await profilesService.VoteAsync(profile1!.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile2!.Id
        });
        await profilesService.VoteAsync(profile2.Id, new VoteRequest
        {
            Liked = true,
            ProfileId = profile1.Id
        });
        var conversation = await unitOfWork.ConversationRepository.InsertAsync(new Conversation());
        await unitOfWork.ParticipantRepository.InsertAsync(new Participant
        {
            ConversationId = conversation.Id,
            ProfileId = profile1.Id
        });
        await unitOfWork.ParticipantRepository.InsertAsync(new Participant
        {
            ConversationId = conversation.Id,
            ProfileId = profile2.Id
        });

        // Act
        var result = await profilesService.GetMatchedAsync(profile1.Id, new PaginationRequest
        {
            Offset = 0,
            Limit = 10,
        });

        // Assert
        Assert.Empty(result.Profiles);
    }
}