using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace IntegrationTests;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "Test";
    private readonly IUnitOfWork _unitOfWork;
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IUnitOfWork unitOfWork)
        : base(options, logger, encoder, clock)
    {
        _unitOfWork = unitOfWork;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var profile = _unitOfWork.ProfileRepository.GetAll().First(x => x.User!.Email == "testEmail");
        var claims = new List<Claim>
        {
            new Claim("UserProfileId", profile.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}