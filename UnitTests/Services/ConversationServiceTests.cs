using AutoMapper;
using Infrastructure.Mappings;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace UnitTests.Services;

public class ConversationServiceTests
{
    private readonly IMapper _mapper;

    public ConversationServiceTests()
    {
        var mockMapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new DtoMappings());
        });
        _mapper = mockMapper.CreateMapper();
    }

    [Fact]
    public async Task GetConversationsAsync_ReturnsNotNullCollection()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var conversationService = new ConversationService(unitOfWork, _mapper);
        var profile1 = await unitOfWork.ProfileRepository.GetAll().FirstAsync();
        var profile2 = await unitOfWork.ProfileRepository.GetAll().Skip(1).FirstAsync();

        // Act
        var conversations = await conversationService.GetConversationsAsync(profile1!.Id, 0, 10);

        // Assert
        Assert.NotNull(conversations);
    }
}
