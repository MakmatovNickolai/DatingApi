using Common.Models;
using Infrastructure.Models.Request;
using Infrastructure.Models.Response;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DatingApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/profiles")]
[Authorize]
[ApiVersion("1.0")]
public class ProfileController : ControllerBase
{
    private readonly IProfilesService _profilesService;

    public ProfileController(IProfilesService profilesService)
    {
        _profilesService = profilesService;
    }

    /// <summary>
    /// Returns 10 closest profiles to user
    /// </summary>
    [HttpPost("explore")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ExploreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Explore([FromBody] ExploreRequest request)
    {
        var userId = User.GetProfileId();
        var profiles = await _profilesService.GetClosestAsync(userId, request);
        var response = new ExploreResponse
        {
            Success = true,
            Profiles = profiles
        };
        return Ok(response);
    }

    /// <summary>
    /// Vote for profile
    /// </summary>
    [HttpPost("vote")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(VoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Vote([FromBody] VoteRequest request)
    {
        var profileId = User.GetProfileId();
        var isMutual = await _profilesService.VoteAsync(profileId, request);
        var response = new VoteResponse
        {
            Success = true,
            IsMutual = isMutual
        };
        return Ok(response);
    }

    /// <summary>
    /// Returns profiles who liked current user
    /// </summary>
    [HttpGet("liked-me")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(LikedMeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetAllLiked([FromQuery] Infrastructure.Models.Request.PaginationRequest request)
    {
        var profileId = User.GetProfileId();
        var profiles = await _profilesService.GetLikedAsync(profileId, request);
        var response = new LikedMeResponse
        {
            Success = true,
            Profiles = profiles
        };
        return Ok(response);
    }

    /// <summary>
    /// Returns matches
    /// </summary>
    [HttpGet("matches")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(MatchesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetMatches([FromQuery] Infrastructure.Models.Request.PaginationRequest request)
    {
        var profileId = User.GetProfileId();
        var profiles = await _profilesService.GetMatchedAsync(profileId, request);
        var response = new MatchesResponse
        {
            Success = true,
            Matches = profiles
        };
        return Ok(response);
    }
}