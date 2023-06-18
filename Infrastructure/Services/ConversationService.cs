using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using DataAccess.Entities;
using DataAccess.Repositories;
using Infrastructure.Models;
using Infrastructure.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public interface IConversationService
{
    Task<List<ConversationDto>> GetConversationsAsync(Guid profileId, int offset, int limit);
    Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, int offset, int limit);
    Task AddMessageAsync(Guid conversationId, Guid profileId, string text);
    Task DeleteConversationAsync(Guid conversationId);
    Task DeleteMessageAsync(Guid messageId);
    Task<Guid> CreateConversationAsync(Guid profileId, Guid targetId);
}

public class ConversationService : IConversationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task AddMessageAsync(Guid conversationId, Guid profileId, string text)
    {
        var conversation = await _unitOfWork.ConversationRepository.GetAsync(conversationId);
        if (conversation == null)
        {
            throw new BadRequestException("Conversation not found");
        }
        await _unitOfWork.MessageRepository.InsertAsync(new Message
        {
            ConversationId = conversationId,
            SenderId = profileId,
            Text = text
        });
    }

    public async Task DeleteConversationAsync(Guid conversationId)
    {
        var conversation = await _unitOfWork.ConversationRepository.GetAsync(conversationId);
        if (conversation == null)
        {
            throw new BadRequestException("Conversation not found");
        }
        await _unitOfWork.ConversationRepository.DeleteAsync(conversationId);
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = _unitOfWork.MessageRepository.GetAsync(messageId);
        if (message == null)
        {
            throw new BadRequestException("Message not found");
        }
        await _unitOfWork.MessageRepository.DeleteAsync(messageId);
    }

    public async Task<List<ConversationDto>> GetConversationsAsync(Guid profileId, int offset, int limit)
    {
        return await _unitOfWork.ConversationRepository.GetAll(x => x.Participants, x => x.Messages)
            .Where(x => x.Participants.Any(p => p.ProfileId == profileId))
            .Skip(offset)
            .Take(limit)
            .Select(x => new ConversationDto
            {
                Id = x.Id,
                CreatedOn = x.CreatedOn,
                LastMessage = _mapper.Map<MessageDto>(x.Messages
                    .OrderByDescending(m => m.CreatedOn)
                    .FirstOrDefault()),
                Profile = _mapper.Map<ProfileDto>(x.Participants.First(p => p.ProfileId != profileId).Profile)
            })
            .ToListAsync();
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid conversationId, int offset, int limit)
    {
        return await _unitOfWork.MessageRepository.GetAll()
            .Where(x => x.ConversationId == conversationId)
            .Skip(offset)
            .Take(limit)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Guid> CreateConversationAsync(Guid profileId, Guid targetId)
    {
        var profileLiked = await _unitOfWork.VoteRepository
            .CountAsync(x => x.VoterId == profileId && x.ReceiverId == targetId && x.IsLiked) > 0;

        var targetLiked = await _unitOfWork.VoteRepository
            .CountAsync(x => x.VoterId == targetId && x.ReceiverId == profileId && x.IsLiked) > 0;

        var isMutualLike = profileLiked && targetLiked;
        if (!isMutualLike)
        {
            throw new BadRequestException("Something went wrong");
        }

        var conversationExists = await _unitOfWork.ParticipantRepository
            .GetAll(x => x.Conversation)
            .Where(x => x.ProfileId == profileId)
            .AnyAsync(x => x.Conversation.Participants.Any(p => p.ProfileId == targetId));

        if (!conversationExists)
        {
            var conversation = await _unitOfWork.ConversationRepository.InsertAsync(new Conversation());
            await _unitOfWork.ParticipantRepository.InsertAsync(new Participant()
            {
                ConversationId = conversation.Id,
                ProfileId = profileId
            });
            await _unitOfWork.ParticipantRepository.InsertAsync(new Participant()
            {
                ConversationId = conversation.Id,
                ProfileId = targetId
            });
            return conversation.Id;
        }
        else
        {
            throw new BadRequestException("Conversation already exists");
        }
    }
}
