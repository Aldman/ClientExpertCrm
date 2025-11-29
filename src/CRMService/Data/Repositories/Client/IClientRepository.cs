namespace CRMService.Data.Repositories.Client;

public interface IClientRepository
{
    Task AddAsync(Models.Client client, CancellationToken cancellationToken);
    Task UpdateAsync(Models.Client client, CancellationToken cancellationToken);
    Task<Models.Client?> GetAsync(Guid clientId, CancellationToken cancellationToken);
    Task<List<Models.Client>> GetClientsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<Models.Client>> GetClientsByPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task DeleteAsync(Guid clientId, CancellationToken cancellationToken);
    void Delete(Models.Client client);
    Task SaveChangesAsync(CancellationToken ct);
}