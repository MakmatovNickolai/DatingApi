using Auth.Dto;
using Common.Exceptions;
using DataAccess.Repositories;
using Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticateDto authenticateDto);
    Task<AuthenticationResponse> RefreshTokenAsync(TokenDto tokenDto);
    Task RevokeTokenAsync(RevokeTokenDto revokeTokenDto);
}

class AuthenticationService : IAuthenticationService
{
    private readonly IUserValidatorService _userValidatorService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(
        IUserValidatorService userValidatorService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userValidatorService = userValidatorService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticateDto authenticateDto)
    {
        var user = await _userValidatorService.ValidateUser(authenticateDto.Email, authenticateDto.Password);
        if (user == null)
        {
            throw new BadRequestException("No such user");
        }

        if (!user.IsGoogleUser && !user.IsConfirmed)
        {
            throw new UnverifiedException();
        }

        var profile = await _unitOfWork.ProfileRepository.GetAsync(x => x.UserId == user.Id);
        if (profile == null)
        {
            throw new BadRequestException("User has no profile");
        }

        var userInfo = new AuthDto
        {
            Email = user.Email,
            UserId = user.Id,
            UserProfileId = profile.Id,
            Name = profile.Name,
            IsEmailConfirmed = user.IsConfirmed
        };

        var token = _jwtTokenService.GenerateToken(userInfo);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenExpiryDate = DateTime.Now.AddDays(_jwtTokenService.GetRefreshTokenValidityInDays());
        refreshTokenExpiryDate = DateTime.SpecifyKind(refreshTokenExpiryDate, DateTimeKind.Utc);
        await _unitOfWork.UserRepository.UpdateAsync(user.Id, (u, ctx) =>
        {
            u.Entity.RefreshToken = refreshToken;
            u.Entity.RefreshTokenExpiryTime = refreshTokenExpiryDate;
        });

        return new AuthenticationResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            AccessTokenExpiration = token.ValidTo,
            RefreshTokenExpiration = refreshTokenExpiryDate,
            User = userInfo
        };
    }

    public async Task<AuthenticationResponse> RefreshTokenAsync(TokenDto tokenDto)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var userNameIdentifier = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userNameIdentifier, out var userId))
        {
            throw new BadRequestException("Token has invalid user id");
        }
        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new UnauthorizedAccessException();
        }

        var userProfile = await _unitOfWork.ProfileRepository.GetAsync(x => x.UserId == user.Id);
        if (userProfile == null)
        {
            throw new BadRequestException("User has no profile");
        }

        if (user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new BadRequestException("Invalid access token or refresh token");
        }

        var userInfo = new AuthDto
        {
            Email = user.Email,
            UserId = user.Id,
            UserProfileId = userProfile.Id,
            Name = userProfile.Name,
            IsEmailConfirmed = user.IsConfirmed
        };

        var newToken = _jwtTokenService.GenerateToken(userInfo);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenExpiryDate = DateTime.Now.AddDays(_jwtTokenService.GetRefreshTokenValidityInDays());
        refreshTokenExpiryDate = DateTime.SpecifyKind(refreshTokenExpiryDate, DateTimeKind.Utc);
        await _unitOfWork.UserRepository.UpdateAsync(user.Id, (u, ctx) =>
        {
            u.Entity.RefreshToken = newRefreshToken;
            u.Entity.RefreshTokenExpiryTime = refreshTokenExpiryDate;
        });

        return new AuthenticationResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newToken),
            RefreshToken = newRefreshToken,
            AccessTokenExpiration = newToken.ValidTo,
            RefreshTokenExpiration = refreshTokenExpiryDate,
            User = userInfo
        };
    }

    public async Task RevokeTokenAsync(RevokeTokenDto revokeTokenDto)
    {
        var user = await _unitOfWork.UserRepository.GetAsync(x => x.RefreshToken == revokeTokenDto.RefreshToken);
        if (user == null)
        {
            throw new BadRequestException("User with refresh token not found");
        }

        await _unitOfWork.UserRepository.UpdateAsync(user.Id, (u, ctx) =>
        {
            u.Entity.RefreshToken = null;
        });
    }
}
