using AutoMapper;
using Common.Exceptions;
using Infrastructure.Mappings;
using Infrastructure.Models.Dto;
using Infrastructure.Models.Request;
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
    public async Task UpdateLocationAsync_UpdatesLocation()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var data = new UpdateLocationRequest
        {
            Latitude = 1,
            Longitude = 2
        };

        // Act
        await userService.UpdateLocationAsync(profile1.Id, data);

        // Assert
        var updatedProfile = await unitOfWork.ProfileRepository.GetAsync(profile1.Id);
        Assert.Equal(data.Latitude, updatedProfile.Latitude);
        Assert.Equal(data.Longitude, updatedProfile.Longitude);
    }


    [Fact]
    public async Task GetProfileDetailsAsync_ReturnsValidInfo_WhenProfileExists()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var user = await unitOfWork.UserRepository.GetAll().FirstAsync();

        // Act
        var info = await userService.GetDetailsAsync(user.Id);

        // Assert
        Assert.NotNull(info);
        Assert.Equal(user.Id, info.Id);
    }

    [Fact]
    public async Task UpdateUserDetailsAsync_ThrowsBadRequestException_WhenNonExistingUser()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var data = new UserDetailUpdateRequest
        {
            Name = "new name"
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => userService.UpdateUserDetailsAsync(Guid.Empty, data));
    }

    //[Fact] todo check again when image validation is done
    public async Task UpdateUserDetailsAsync_ThrowsBadRequestException_WhenNonExistingImages()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile = await unitOfWork.ProfileRepository.GetAll().FirstOrDefaultAsync();
        var images = new ImageDto[]
        {
            new ImageDto
            {
                Id = Guid.Empty,
                OrderIndex = 0
            },
            new ImageDto
            {
                Id = Guid.NewGuid(),
                OrderIndex = 1
            }
        };
        var data = new UserDetailUpdateRequest
        {
            Name = "new name",
            DateOfBirth = "11.11.1111",
            Images = images
        };

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => userService.UpdateUserDetailsAsync(profile!.Id, data));
    }

    [Fact]
    public async Task UpdateUserDetailsAsync_UpdatesUserDetails()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var image = await unitOfWork.ImageRepository.GetAll().FirstAsync();
        var images = new ImageDto[]
        {
            new ImageDto
            {
                Id = image.Id,
                OrderIndex = 0
            }
        };
        var data = new UserDetailUpdateRequest
        {
            Name = "new name",
            DateOfBirth = "11.11.1111",
            Images = images
        };

        // Act
        await userService.UpdateUserDetailsAsync(profile.Id, data);

        // Assert
        var updatedProfile = await unitOfWork.ProfileRepository.GetAll(x => x.Images).Where(x => x.Id == profile.Id).FirstAsync();
        Assert.Equal(data.Name, updatedProfile.Name);
        Assert.Contains(image.Id, updatedProfile.Images.Select(i => i.Id));
    }

    [Fact]
    public async Task UpdateSearchSettings_UpdatesSettings()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var user = await unitOfWork.UserRepository.GetAll().FirstAsync();
        var request = new UpdateSearchSettingsRequest
        {
            AgeMin = 20,
            AgeMax = 21,
            Distance = 20
        };

        // Act
        await userService.UpdateSearchSettingsAsync(user.Id, request);

        // Assert
        var updatedUser = await unitOfWork.UserRepository.GetAsync(user.Id);
        Assert.Equal(request.AgeMin, updatedUser.FilterAgeMin);
        Assert.Equal(request.AgeMax, updatedUser.FilterAgeMax);
        Assert.Equal(request.Distance, updatedUser.FilterDistance);
    }

    [Fact]
    public async Task UpdateUserDetailsAsync_ParsesAndUpdatesStringDateOfBirth()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var userService = new UserService(_mapper, unitOfWork, _userValidatorServiceMock.Object);
        var profile = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var images = new ImageDto[0]
        {

        };
        var request = new UserDetailUpdateRequest
        {
            Name = "new name",
            DateOfBirth = "11.11.1111",
            Images = images
        };

        // Act
        await userService.UpdateUserDetailsAsync(profile.Id, request);

        // Assert
        var updatedProfile = await unitOfWork.ProfileRepository.GetAsync(profile.Id);
        Assert.Equal(new DateOnly(1111, 11, 11), updatedProfile.BirthDate);
    }
}