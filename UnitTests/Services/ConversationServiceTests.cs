using AutoMapper;
using Infrastructure.Mappings;
using Infrastructure.Services;
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
    public async Task GetConversationsAsync_ReturnsValidInfo()
    {
        // Arrange
        using var unitOfWork = new UnitOfWorkFixture(DateTime.Now.Ticks.ToString()).UnitOfWork;
        var conversationService = new ConversationService(unitOfWork, _mapper);
        var user1 = await unitOfWork.UserRepository.GetAsync(x => x.Email == "test1");
        var user2 = await unitOfWork.UserRepository.GetAsync(x => x.Email == "test2");
        var profile1 = await unitOfWork.ProfileRepository.GetAsync(x => x.UserId == user1!.Id);
        var profile2 = await unitOfWork.ProfileRepository.GetAsync(x => x.UserId == user2!.Id);
        var message = unitOfWork.MessageRepository.GetAll().First();

        // Act
        var conversations = await conversationService.GetConversationsAsync(profile1!.Id, 0, 10);

        // Assert
        Assert.NotEmpty(conversations);
        conversations.ForEach(conversation =>
        {
            Assert.NotNull(conversation.Profile);
            Assert.Equal(profile2!.Id, conversation.Profile.Id);
            Assert.NotNull(conversation.LastMessage);
            Assert.Equal(message.Id, conversation.LastMessage.Id);
        });
    }
}
