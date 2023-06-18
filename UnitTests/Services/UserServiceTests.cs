using AutoMapper;
using Common.Exceptions;
using Infrastructure.Mappings;
using Infrastructure.Models;
using Infrastructure.Models.Users;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests.Services;

// todo seems like not really a unit test
public class UserServiceTests
{
    private readonly Mock<IUserValidatorService> _userValidatorServiceMock;
    private readonly IMapper _mapper;

    public UserServiceTests()
    {
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DtoMappings());
        });
        _mapper = mockMapper.CreateMapper();
        _userValidatorServiceMock = new Mock<IUserValidatorService>();
    }

    [Fact]
    public async Task DeleteAsync_DeletesUser()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var user = await unitOfWork.UserRepository.GetAll().FirstOrDefaultAsync();

        // Act
        await userService.DeleteAsync(user!.Id);

        // Assert
        var deletedUser = await unitOfWork.UserRepository.GetAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task VoteAsync_CreatesARecordInDatabase()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await userService.VoteAsync(profile1!.Id, new VoteDto
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
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        var firstLikedSecond = await userService.VoteAsync(profile1!.Id, new VoteDto
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        var secondLikedFirst = await userService.VoteAsync(profile2.Id, new VoteDto
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
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        var firstLikedSecond = await userService.VoteAsync(profile1!.Id, new VoteDto
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
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();

        // Act
        try
        {
            await userService.VoteAsync(profile1!.Id, new VoteDto
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
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);

        // Act
        await userService.VoteAsync(profile1!.Id, new VoteDto
        {
            ProfileId = profile2!.Id,
            Liked = true
        });
        try
        {
            await userService.VoteAsync(profile1.Id, new VoteDto
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
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id);
        var profile3 = await unitOfWork.ProfileRepository.GetAsync(x => x.Id != profile1.Id && x.Id != profile2!.Id);

        var firstLikedSecond = await userService.VoteAsync(profile1.Id, new VoteDto
        {
            Liked = true,
            ProfileId = profile2!.Id
        });
        var secondLikedFirst = await userService.VoteAsync(profile2.Id, new VoteDto
        {
            Liked = true,
            ProfileId = profile1!.Id
        });
        var thirdLikedFirst = await userService.VoteAsync(profile3!.Id, new VoteDto
        {
            Liked = true,
            ProfileId = profile1!.Id
        });

        // Act
        var result = await userService.GetLikedProfilesAsync(profile1.Id, new PaginationRequest
        {
            Offset = 0,
            Limit = 10,
        });

        // Assert
        Assert.True(secondLikedFirst);
        Assert.False(thirdLikedFirst);
        Assert.Single(result.LikedUsers);
    }

    [Fact]
    public async Task GetProfileDetailsAsync_ReturnsValidInfo()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();

        // Act
        var info = await userService.GetProfileDetailsAsync(profile1.Id);

        // Assert
        Assert.NotNull(info);
        Assert.Equal(profile1.Id, info.Id);
        Assert.Equal(profile1.Name, info.Name);
    }

    [Fact]
    public async Task GetClosestProfilesAsync_ReturnsValidCount()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var testLongitude = 10;
        var testLatitude = 10;
        var testRadius = 100;

        // Act
        var closestProfiles = await userService.GetClosestProfilesAsync(profile.Id, new ExploreRequest
        {
            Latitude = testLatitude,
            Longitude = testLongitude,
            Radius = testRadius
        });

        // Assert
        Assert.Equal(10, closestProfiles.Count);
    }
}