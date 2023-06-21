using Auth.Request;
using Auth.Response;
using Auth.Services;
using Common.Exceptions;
using Common.Models;
using DatingApi.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace DatingApi.Controllers;

[ApiController]
// todo add versioning
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
[ValidateModel]
public class AuthController : ControllerBase
{
    private readonly IRegisterService _registerService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IRestorePasswordService _restorePasswordService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IRegisterService registerService,
        IAuthenticationService authenticationService,
        IRestorePasswordService restorePasswordService,
        ILogger<AuthController> logger)
    {
        _registerService = registerService;
        _authenticationService = authenticationService;
        _restorePasswordService = restorePasswordService;
        _logger = logger;
    }

    /// <summary>
    /// Checks if email is in use
    /// </summary>
    [HttpGet("check-email")]
    [ProducesResponseType(typeof(EmailUsageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CheckEmail([FromQuery] CheckEmailRequest request)
    {
        var isInUse = await _registerService.CheckEmailInUseAsync(request.Email);
        var response = new EmailUsageResponse
        {
            Success = true,
            IsInUse = isInUse
        };
        return Ok(response);
    }

    /// <summary>
    /// Checks if google user is in use
    /// </summary>
    [HttpPost("check-google")]
    [ProducesResponseType(typeof(CheckGoogleUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CheckGoogleUser([FromBody] GoogleUserRequest data)
    {
        var checkResult = await _registerService.CheckGoogleUserAsync(data);
        checkResult.Success = true;
        return Ok(checkResult);
    }

    /// <summary>
    /// Checks if email is verified
    /// </summary>
    [HttpPost("check-verification")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(EmailConfirmationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CheckEmailVerification([FromBody] CheckEmailRequest request)
    {
        var isConfirmed = await _registerService.CheckEmailVerificationAsync(request.Email);
        var response = new EmailConfirmationResponse
        {
            Success = true,
            IsConfirmed = isConfirmed
        };
        return Ok(response);
    }

    /// <summary>
    /// Registers a new google user
    /// </summary>
    [HttpPost("register/google")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<SuccessfulResponse>> RegisterGoogle([FromBody] RegisterGoogleRequest request)
    {
        var response = await _registerService.RegisterGoogleAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Registers a new email user
    /// </summary>
    [HttpPost("register/email")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RegisterEmail([FromBody] RegisterEmailRequest request)
    {
        var response = await _registerService.RegisterEmailAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Confirms an email of the user, must not be called from the frontend
    /// </summary>
    [HttpGet("confirm")]
    [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmUserRequest request,
        [FromServices] IOptions<Auth.Config.LinkOptions> linkOptions)
    {
        // todo analyze where this is will be called from
        var result = await _registerService.ConfirmUserAsync(request);
        if (result)
        {
            return Ok(new SuccessfulResponse("Successful verification"));
        }

        throw new BadRequestException("Invalid code");
    }

    /// <summary>
    /// Requests another email verification code, if the code is outdated
    /// </summary>
    [HttpPost("repeat-verification")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Repeat([FromBody] UserEmailRequest repeatPasswordDto)
    {
        await _registerService.RepeatCodeAsync(repeatPasswordDto);
        return Ok(new SuccessfulResponse("Verification link was sent"));
    }

    /// <summary>
    /// Sets new password for the user, using a verification code provided
    /// </summary>
    [HttpPost("set-password")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SetNewPassword([FromBody] SetNewPasswordRequest request)
    {
        await _restorePasswordService.SetNewPasswordAsync(request);
        return Ok(new SuccessfulResponse("New password was set"));
    }

    /// <summary>
    /// Issues a Bearer Access token
    /// </summary>
    [HttpPost("authenticate")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request)
    {
        AuthenticationResponse response = await _authenticationService.AuthenticateAsync(request);
        response.Success = true;
        return Ok(response);
    }

    /// <summary>
    /// Sends an email with restore password link
    /// </summary>
    [HttpPost("restore")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ResetPassword([FromBody] UserEmailRequest request)
    {
        await _restorePasswordService.ForgotPasswordAsync(request);
        return Ok(new SuccessfulResponse("Email with restore link was sent"));
    }

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenDto)
    {
        AuthenticationResponse response = await _authenticationService.RefreshTokenAsync(tokenDto);
        response.Success = true;
        return Ok(response);
    }

    /// <summary>
    /// Revokes a refresh token
    /// </summary>
    [HttpPost("revoke-token")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(SuccessfulResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request)
    {
        // todo not only remove from db, but invalidate further access token requests
        await _authenticationService.RevokeTokenAsync(request);
        return Ok(new SuccessfulResponse("Token was revoked"));
    }
}