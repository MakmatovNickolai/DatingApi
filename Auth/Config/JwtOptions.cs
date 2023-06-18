using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth.Config;

public class JwtOptions
{
    public string SecretKey { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int TTL { get; set; }
    public int RefreshTokenValidityInDays { get; set; }

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }
}
