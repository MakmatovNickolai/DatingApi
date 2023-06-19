using AutoMapper;
using Infrastructure.Mappings;
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
}