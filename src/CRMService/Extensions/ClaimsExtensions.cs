using System.Security.Claims;
using Shared.Constants;

namespace CRMService.Extensions;

public static class ClaimsExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(WellKnownNames.UserIdHeader)!.ToGuid();
    }
}