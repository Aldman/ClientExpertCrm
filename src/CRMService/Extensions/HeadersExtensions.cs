using Shared.Constants;

namespace CRMService.Extensions;

public static class HeadersExtensions
{
    public static Guid GetUserId(this IHeaderDictionary headers)
    {
        return headers[WellKnownNames.UserIdHeader]
            .ToString()
            .ToGuid();
    }
}