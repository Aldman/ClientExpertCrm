using CRMService.DTOs.Session;

namespace CRMService.Cache;

public interface ICacheRepository
{
    void AddOrUpdate(string hashKey, string sessionKey, string value);
    void AddOrUpdate(SessionResponseDto session);
    string? Get(string hashKey, string sessionKey);
    IEnumerable<string> GetAll(string hashKey);
    IEnumerable<SessionResponseDto> GetAllSerialized(string hashKey);
    void DeleteEntity(string hashKey, string sessionKey);
}