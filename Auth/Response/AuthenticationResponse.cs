using Auth.Dto;
using Common.Models;

namespace Auth.Response;

public class AuthenticationResponse : BaseResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public AuthDto User { get; set; }
}