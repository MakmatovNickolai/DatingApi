using Auth.Config;
using Auth.Dto;
using Auth.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Services;

public interface IJwtTokenService
{
    JwtSecurityToken GenerateToken(AuthDto auth);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
    int GetRefreshTokenValidityInDays();
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtTokenService> _logger;
    public JwtTokenService(IOptions<JwtOptions> jwtOptions, ILogger<JwtTokenService> logger)
    {
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
    }

    public JwtSecurityToken GenerateToken(AuthDto auth)
    {
        var claims = new[]
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, auth.Email),
            new Claim(ClaimTypes.Email, auth.Email),
            new Claim(ClaimTypes.NameIdentifier, auth.UserId.ToString()),
            new Claim("UserProfileId", auth.UserProfileId.ToString()),
            new Claim(LocalClaimTypes.FirstName, auth.Name),
            new Claim(LocalClaimTypes.IsConfirmed, auth.IsEmailConfirmed.ToString().ToLower())
        };

        var expires = DateTime.UtcNow.AddSeconds(_jwtOptions.TTL);

        var jwt = new JwtSecurityToken(
            notBefore: DateTime.UtcNow,
            claims: claims,
            expires: expires,
            audience: _jwtOptions.Audience,
            issuer: _jwtOptions.Issuer,
            signingCredentials: new SigningCredentials(_jwtOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return jwt;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal? principal;
        SecurityToken? securityToken;
        try
        {
            principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error validating token: {ex.Message}");
            throw new SecurityTokenException("Invalid token");
        }

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }

    public int GetRefreshTokenValidityInDays()
    {
        return _jwtOptions.RefreshTokenValidityInDays;
    }
}
