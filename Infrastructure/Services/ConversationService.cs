using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Exceptions;
using DataAccess.Entities;
using DataAccess.Repositories;
using Infrastructure.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public interface IConversationService
{
    Task<ConversationsDto> GetConversationsAsync(Guid profileId, int offset, int limit);
    Task<MessagesDto> GetMessagesAsync(Guid conversationId, int lastMessageId, string lastCreatedOn, int limit);
    Task<int> AddMessageAsync(Guid conversationId, Guid senderId, string text);
    Task MarkMessageAsReadAsync(Guid readerId, int messageId, Guid conversationId);
    Task DeleteConversationAsync(Guid conversationId);
    Task DeleteMessageAsync(int messageId);
    Task<Guid> CreateConversationAsync(Guid profileId, Guid targetId);
    Task<Guid> GetReceiverIdAsync(Guid conversationId, Guid senderId);
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

    public async Task<int> AddMessageAsync(Guid conversationId, Guid senderId, string text)
    {
        var conversation = await _unitOfWork.ConversationRepository.GetAsync(conversationId);
        if (conversation == null)
        {
            throw new BadRequestException("Conversation not found");
        }
        var message = await _unitOfWork.MessageRepository.InsertAsync(new Message
        {
            ConversationId = conversationId,
            SenderId = senderId,
            Text = text
        });

        return message.Id;
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

    public async Task DeleteMessageAsync(int messageId)
    {
        var message = _unitOfWork.MessageRepository.GetAsync(messageId);
        if (message == null)
        {
            throw new BadRequestException("Message not found");
        }
        await _unitOfWork.MessageRepository.DeleteAsync(messageId);
    }

    public async Task<ConversationsDto> GetConversationsAsync(Guid profileId, int offset, int limit)
    {
        var totalCount = await _unitOfWork.ConversationRepository.GetAll()
            .CountAsync(x => x.Participants.Any(p => p.ProfileId == profileId)); // todo skip with blocked users
        var conversations = await _unitOfWork.ConversationRepository.GetAll()
            .Include(x => x.Participants)
                .ThenInclude(x => x.Profile)
                .ThenInclude(x => x.Gender)
            .Include(x => x.Participants)
                .ThenInclude(x => x.Profile)
                .ThenInclude(x => x.Images)
            .Where(x => x.Participants.Any(p => p.ProfileId == profileId))
            //.OrderByDescending(x => x.Messages.OrderByDescending(m => m.CreatedOn).FirstOrDefault().CreatedOn) // todo
            .Skip(offset)
            .Take(limit)
            .Select(x => new ConversationDto
            {
                Id = x.Id,
                CreatedOn = x.CreatedOn.ToString("o"),
                LastMessage = _mapper.Map<MessageDto>(x.Messages
                    .OrderByDescending(m => m.CreatedOn)
                    .FirstOrDefault()),
                Profile = _mapper.Map<ProfileDto>(x.Participants.First(p => p.ProfileId != profileId).Profile) // todo gender is null
            })
            .ToListAsync();

        return new ConversationsDto
        {
            Conversations = conversations,
            TotalCount = totalCount
        };
    }

    public async Task MarkMessageAsReadAsync(Guid readerId, int messageId, Guid conversationId)
    {
        var conversation = await _unitOfWork.ConversationRepository.GetAll(x => x.Participants).FirstOrDefaultAsync(x => x.Id == conversationId);
        if (conversation == null)
        {
            throw new BadRequestException("Wrong conversation");
        }
        var isParticipant = conversation.Participants.Any(x => x.ProfileId == readerId);
        if (!isParticipant)
        {
            throw new BadRequestException("Not a participant");
        }
        var message = await _unitOfWork.MessageRepository.GetAsync(messageId);
        if (message != null)
        {
            await _unitOfWork.MessageRepository.UpdateAsync(message.Id, (u, ctx) =>
            {
                u.Entity.IsRead = true;
            });
        }
    }

    public async Task<MessagesDto> GetMessagesAsync(Guid conversationId, int lastMessageId, string lastCreatedOn, int limit)
    {
        bool isInitialRequest = string.IsNullOrEmpty(lastCreatedOn);
        if (isInitialRequest)
        {
            var initialMessages = await _unitOfWork.MessageRepository.GetAll()
                .Where(x => x.ConversationId == conversationId)
                .OrderByDescending(x => x.CreatedOn)
                .Take(limit)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var lastItem = initialMessages.LastOrDefault();

            return new MessagesDto
            {
                Messages = initialMessages,
                LastMessageId = lastItem?.Id ?? 0,
                LastMessageCreatedOn = lastItem?.CreatedOn ?? string.Empty
            };
        }
        else
        {
            var lastCreatedOnDateTime = DateTime.Parse(lastCreatedOn);
            lastCreatedOnDateTime = DateTime.SpecifyKind(lastCreatedOnDateTime, DateTimeKind.Utc);
            var moreMessages = await _unitOfWork.MessageRepository.GetAll()
                .Where(x => x.ConversationId == conversationId && x.CreatedOn < lastCreatedOnDateTime && x.Id < lastMessageId)
                .OrderByDescending(x => x.CreatedOn)
                .Take(limit)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var lastItem = moreMessages.LastOrDefault();

            return new MessagesDto
            {
                Messages = moreMessages,
                LastMessageId = lastItem?.Id ?? 0,
                LastMessageCreatedOn = lastItem?.CreatedOn ?? string.Empty
            };
        }
    }

    public async Task<Guid> CreateConversationAsync(Guid profileId, Guid targetId)
    {
        var isMutualLike = await _unitOfWork.VoteRepository
            .CountAsync(x => x.VoterId == profileId && x.ReceiverId == targetId && x.IsMatched) > 0;
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

    public async Task<Guid> GetReceiverIdAsync(Guid conversationId, Guid senderId)
    {
        return await _unitOfWork.ConversationRepository.GetAll()
            .Where(x => x.Id == conversationId)
            .Select(x => x.Participants.First(p => p.ProfileId != senderId).ProfileId)
            .FirstAsync();
    }
}