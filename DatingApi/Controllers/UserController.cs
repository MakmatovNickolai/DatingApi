using Auth.Dto;
using Auth.Services;
using Common.Models;
using Infrastructure.Models;
using Infrastructure.Models.Users;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace DatingApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/users/me")]
[Authorize]
[ApiVersion("1.0")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRegisterService _registerService;

    public UserController(IUserService userService, IRegisterService registerService)
    {
        _userService = userService;
        _registerService = registerService;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetails()
    {
        var profileId = User.GetProfileId();
        var profile = await _userService.GetProfileDetailsAsync(profileId);
        var response = new ProfileResponse
        {
            Success = true,
            Profile = profile
        };
        return Ok(response);
    }

    /// <summary>
    /// Updates an info about user
    /// </summary>
    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateUserDetails([FromBody] UserDetailUpdateRequest data)
    {
        var userId = User.GetId();
        await _userService.UpdateUserDetailsAsync(userId, data);
        if (!string.IsNullOrEmpty(data.Password))
        {
            await _registerService.SetPasswordAsync(new SetPasswordDto(userId, data.Password));
        }

        return Ok(new SuccessfulResponse("User updated"));
    }

    /// <summary>
    /// Deletes a current user
    /// </summary>
    [HttpDelete]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Delete()
    {
        await _userService.DeleteAsync(User.GetId());
        return Ok(new SuccessfulResponse("User deleted"));
    }

    /// <summary>
    /// Updates user's location
    /// </summary>
    [HttpPost("location")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto data)
    {
        var profileId = User.GetProfileId();
        await _userService.UpdateLocationAsync(profileId, data);
        return Ok(new SuccessfulResponse("Location updated"));
    }
}
