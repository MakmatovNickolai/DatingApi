using Common.Models;
using DatingApi.SignalR;
using Infrastructure.Models.Request;
using Infrastructure.Models.Response;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using PaginationRequest = Infrastructure.Models.Request.PaginationRequest;

namespace DatingApi.Controllers;

// todo change namings
[ApiController]
[Route("api/v{version:apiVersion}/conversations")]
[Authorize]
[ApiVersion("1.0")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly HubContextService _hubContextService;

    public ConversationController(
        IConversationService conversationService, HubContextService hubContextService)
    {
        _conversationService = conversationService;
        _hubContextService = hubContextService;
    }

    /// <summary>
    /// Returns conversations of the user
    /// </summary>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ConversationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetConversations([FromQuery] PaginationRequest request)
    {
        var profileId = User.GetProfileId();
        var result = await _conversationService.GetConversationsAsync(profileId, request.Offset, request.Limit);
        var response = new ConversationsResponse
        {
            Success = true,
            Conversations = result
        };
        return Ok(response);
    }

    /// <summary>
    /// Sends a message to a conversation
    /// </summary>
    [HttpPost("send-message")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SendMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        var senderId = User.GetProfileId();
        var messageId = await _conversationService.AddMessageAsync(request.ConversationId, senderId, request.Text);
        var receiverId = await _conversationService.GetReceiverIdAsync(request.ConversationId, senderId);
        await _hubContextService.SendMessageAsync(senderId.ToString(), request.ConversationId, request.Text); // todo change to receiverId
        var response = new SendMessageResponse
        {
            Success = true,
            MessageId = messageId
        };
        return Ok(response);
    }

    /// <summary>
    /// Returns messages in a conversation
    /// </summary>
    [HttpPost("messages")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MessagesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetMessages([FromBody] MessagesRequest request)
    {
        var messages = await _conversationService.GetMessagesAsync(request.ConversationId,
            request.LastMessageId, request.LastMessageCreatedOn, request.Limit);
        var response = new MessagesResponse
        {
            Success = true,
            Response = messages
        };
        return Ok(response);
    }

    /// <summary>
    /// Creates a conversation
    /// </summary>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(CreateConversationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request)
    {
        var profileId = User.GetProfileId();
        var conversationId = await _conversationService.CreateConversationAsync(profileId, request.ReceiverId);
        var response = new CreateConversationResponse
        {
            Success = true,
            ConversationId = conversationId
        };
        return Ok(response);
    }

    /// <summary>
    /// Marks unread messages as read
    /// </summary>
    [HttpPost("read-messages")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead([FromBody] MarkAsReadRequest request)
    {
        var senderId = User.GetProfileId();
        await _conversationService.MarkMessagesAsReadAsync(senderId, request.LastMessageId, request.ConversationId);
        var receiverId = await _conversationService.GetReceiverIdAsync(request.ConversationId, senderId);
        await _hubContextService.MarkMessagesAsReadAsync(senderId.ToString(), request.ConversationId, request.LastMessageId); // todo change to receiverId
        return Ok(new SuccessfulResponse("Ok"));
    }
}