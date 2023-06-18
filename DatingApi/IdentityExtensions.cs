using System.Security.Claims;

namespace DatingApi;

public static class IdentityExtensions
{
    public static Guid GetId(this ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(idClaim, out var result))
        {
            return result;
        }

        return Guid.Empty;
    }

    public static Guid GetProfileId(this ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirstValue("UserProfileId");

        if (Guid.TryParse(idClaim, out var result))
        {
            return result;
        }

        return Guid.Empty;
    }
}
