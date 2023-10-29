using Auth.Dto;
using Auth.Services;
using Common.Models;
using Infrastructure.Models.Request;
using Infrastructure.Models.Response;
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


    /// <summary>
    /// Returns user's details
    /// </summary>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetails()
    {
        var userId = User.GetId();
        var userDto = await _userService.GetDetailsAsync(userId);
        var response = new UserResponse
        {
            Success = true,
            User = userDto
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
    public async Task<IActionResult> UpdateUserDetails([FromBody] UserDetailUpdateRequest request)
    {
        var profileId = User.GetProfileId();
        var userId = User.GetId();
        await _userService.UpdateUserDetailsAsync(profileId, request);
        if (!string.IsNullOrEmpty(request.Password))
        {
            await _registerService.SetPasswordAsync(new SetPasswordDto
            {
                UserId = userId,
                Password = request.Password
            });
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
    public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationRequest request)
    {
        var profileId = User.GetProfileId();
        await _userService.UpdateLocationAsync(profileId, request);
        return Ok(new SuccessfulResponse("Location updated"));
    }

    /// <summary>
    /// Updates user's search settings
    /// </summary>
    [HttpPost("search")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateSearchSettings([FromBody] UpdateSearchSettingsRequest request)
    {
        var userId = User.GetId();
        await _userService.UpdateSearchSettingsAsync(userId, request);
        return Ok(new SuccessfulResponse("Settings are updated"));
    }
}
