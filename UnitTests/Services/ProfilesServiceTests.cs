using AutoMapper;
using Common.Exceptions;

using Infrastructure.Mappings;
using Infrastructure.Models;
using Infrastructure.Models.Users;
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
        var profileService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await profileService.VoteAsync(profile1!.Id, new VoteDto
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
        var profileService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        var firstLikedSecond = await profileService.VoteAsync(profile1!.Id, new VoteDto
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        var secondLikedFirst = await profileService.VoteAsync(profile2.Id, new VoteDto
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
        var profileService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        var firstLikedSecond = await profileService.VoteAsync(profile1!.Id, new VoteDto
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
        var profileService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();

        // Act
        try
        {
            await profileService.VoteAsync(profile1!.Id, new VoteDto
            {
                ProfileId = Guid.Empty,
                Liked = true
            });
            Assert.True(false);
        }
        catch (Exception ex)
        {
            // Assert
            Assert.IsType<BadRequestException>(ex);
        }
    }

    [Fact]
    public async Task VoteAsync_ThrowsDuplicatedObjectException_WhenVotingAlreadyVotedTarget()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profileService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await profileService.VoteAsync(profile1!.Id, new VoteDto
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        try
        {
            await profileService.VoteAsync(profile1.Id, new VoteDto
            {
                ProfileId = profile2.Id,
                Liked = false
            });
            Assert.True(false);
        }
        catch (Exception ex)
        {
            // Assert
            Assert.IsType<DuplicatedObjectException>(ex);
        }
    }

    [Fact]
    public async Task GetLikedProfilesAsync_ReturnsNonMutualLikes()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var profileService = new ProfilesService(_mapper, unitOfWork);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);
        var profile3 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id && x.Id != profile2!.Id);

        var firstLikedSecond = await profileService.VoteAsync(profile1.Id, new VoteDto
        {
            Liked = true,
            ProfileId = profile2!.Id
        });
        var secondLikedFirst = await profileService.VoteAsync(profile2.Id, new VoteDto
        {
            Liked = true,
            ProfileId = profile1!.Id
        });
        var thirdLikedFirst = await profileService.VoteAsync(profile3!.Id, new VoteDto
        {
            Liked = true,
            ProfileId = profile1!.Id
        });

        // Act
        var result = await profileService.GetLikedAsync(profile1.Id, new PaginationRequest
        {
            Offset = 0,
            Limit = 10,
        });

        // Assert
        Assert.True(secondLikedFirst);
        Assert.False(thirdLikedFirst);
        Assert.Single(result.LikedUsers);
    }
}