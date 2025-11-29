using CRMService.Constants;

namespace CRMService.Extensions;

public static class GuidExtensions
{
    public static string ToSessionCacheKey(this Guid guid) => $"{RedisConstants.SessionKeyPrefix}{guid}";
    public static string ToHashKey(this Guid guid) => $"{RedisConstants.HashKeyPrefix}{guid}";
}