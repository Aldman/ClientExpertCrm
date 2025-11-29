namespace CRMService.Data.Repositories.Session;

public interface ISessionRepository
{
    Task AddAsync(Models.Session session, CancellationToken cancellationToken);
    Task UpdateAsync(Models.Session session, CancellationToken cancellationToken);
    Task<Models.Session?> GetAsync(Guid sessionId, CancellationToken cancellationToken);
    Task<List<Models.Session>> GetSessionsByClientIdAsync(Guid clientId, CancellationToken cancellationToken);
    Task<List<Models.Session>> GetSessionsByPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task DeleteAsync(Guid sessionId, CancellationToken cancellationToken);
    void Delete(Models.Session session);
    Task SaveChangesAsync(CancellationToken ct);
}