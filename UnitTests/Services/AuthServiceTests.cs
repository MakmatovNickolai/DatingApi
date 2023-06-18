using Auth.Config;
using Auth.Dto;
using Auth.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<ILogger<JwtTokenService>> _loggerMock;
    private readonly JwtTokenService _sut;

    public AuthServiceTests()
    {
        _loggerMock = new Mock<ILogger<JwtTokenService>>();

        var options = new JwtOptions
        {
            SecretKey = "78f9c6f6-db9e-4a76-9adf-66d5967693f5",
            Audience = "Dating/Api",
            Issuer = "Dating/Api/AccessToken",
            TTL = 3600
        };

        _sut = new JwtTokenService(Options.Create(options), _loggerMock.Object);
    }

    [Fact]
    public void GenerateToken_ReturnsJwt()
    {
        // Arrange
        var auth = new AuthDto
        {
            Email = "test@example.com",
            UserId = Guid.NewGuid(),
            UserProfileId = Guid.NewGuid(),
            Name = "Test User",
            IsEmailConfirmed = true
        };



        // Act
        var result = _sut.GenerateToken(auth);

        // Assert
        Assert.IsType<JwtSecurityToken>(result);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsString()
    {
        // Arrange

        // Act
        var result = _sut.GenerateRefreshToken();

        // Assert
        Assert.IsType<string>(result);
    }
}
