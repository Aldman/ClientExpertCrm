using System.Text.Json;
using CRMService.DTOs.Session;
using CRMService.Extensions;
using StackExchange.Redis;

namespace CRMService.Cache;

public class CacheRepository : ICacheRepository
{
    private readonly IDatabase _db;

    public CacheRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public void AddOrUpdate(string hashKey, string sessionKey, string value)
    {
        _db.HashSet(hashKey, [
            new HashEntry(sessionKey, value)
        ]);
    }

    public void AddOrUpdate(SessionResponseDto session)
    {
        AddOrUpdate(
            hashKey: session.ClientId.ToHashKey(),
            sessionKey: session.Id.ToSessionCacheKey(),
            value: JsonSerializer.Serialize(session)
        );
    }

    public string? Get(string hashKey, string sessionKey)
    {
        return _db.HashGet(hashKey, sessionKey);
    }

    public IEnumerable<string> GetAll(string hashKey)
    {
        return _db.HashGetAll(hashKey)
            .Where(x => x.Value.HasValue)
            .Select(x => x.Value.ToString());
    }

    public IEnumerable<SessionResponseDto> GetAllSerialized(string hashKey)
    {
        return GetAll(hashKey).Select(x => JsonSerializer.Deserialize<SessionResponseDto>(x)!);
    }

    public void DeleteEntity(string hashKey, string sessionKey)
    {
        _db.HashDelete(hashKey, sessionKey);
    }
}