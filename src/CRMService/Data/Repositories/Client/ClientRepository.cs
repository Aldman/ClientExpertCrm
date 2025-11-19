using CRMService.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMService.Data.Repositories.Client;

public class ClientRepository : IClientRepository
{
    private readonly CrmDbContext _context;

    public ClientRepository(CrmDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Models.Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    public async Task UpdateAsync(Models.Client client, CancellationToken cancellationToken)
    {
        var toUpdate = await GetAsync(client.Id, cancellationToken);
        toUpdate?.Change(client);
    }

    public async Task<Models.Client?> GetAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken);
    }

    public async Task<List<Models.Client>> GetClientsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Clients
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Models.Client>> GetClientsByPaginationAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        return await _context.Clients
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid clientId, CancellationToken cancellationToken)
    {
        var client = await GetAsync(clientId, cancellationToken);
        if (client is not null)
            Delete(client);
    }

    public void Delete(Models.Client client)
    {
        _context.Clients.Remove(client);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}