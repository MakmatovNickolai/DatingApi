using Common.Models;
using DatingApi.Hubs;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Mime;
using PaginationRequest = Infrastructure.Models.Users.PaginationRequest;

namespace DatingApi.Controllers;

// todo change namings
[ApiController]
[Route("api/v{version:apiVersion}/conversations")]
[Authorize]
[ApiVersion("1.0")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly IHubContext<ConversationHub> _hubContext;
    public ConversationController(
        IConversationService conversationService,
        IHubContext<ConversationHub> hubContext)
    {
        _conversationService = conversationService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Returns conversations of the user
    /// </summary>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ConversationsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetChatRooms([FromQuery] PaginationRequest request)
    {
        var profileId = User.GetProfileId();
        var conversations = await _conversationService.GetConversationsAsync(profileId, request.Offset, request.Limit);
        var response = new ConversationsResponse
        {
            Success = true,
            Conversations = conversations
        };
        return Ok(response);
    }

    /// <summary>
    /// Sends a message to a conversation
    /// </summary>
    [HttpPost("send-message")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto data)
    {
        var profileId = User.GetProfileId();
        await _conversationService.AddMessageAsync(data.ConversationId, profileId, data.Text);
        await _hubContext.Clients.All.SendAsync("ConversationMessage", data.ConversationId, profileId, data.Text);
        return Ok(new SuccessfulResponse("Message sent"));
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
        var messages = await _conversationService.GetMessagesAsync(request.ConversationId, request.Offset, request.Limit);
        var response = new MessagesResponse
        {
            Success = true,
            Messages = messages
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
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationDto data)
    {
        var profileId = User.GetProfileId();
        var conversationId = await _conversationService.CreateConversationAsync(profileId, data.TargetId);
        var response = new CreateConversationResponse
        {
            Success = true,
            ConversationId = conversationId
        };
        return Ok(response);
    }
}