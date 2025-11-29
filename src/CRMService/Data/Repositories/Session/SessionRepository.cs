using CRMService.Models;
using Microsoft.EntityFrameworkCore;

namespace CRMService.Data.Repositories.Session;

public class SessionRepository : ISessionRepository
{
    private readonly CrmDbContext _context;

    public SessionRepository(CrmDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Models.Session session, CancellationToken cancellationToken)
    {
        await _context.Sessions.AddAsync(session, cancellationToken);
    }

    public async Task UpdateAsync(Models.Session session, CancellationToken cancellationToken)
    {
        var toUpdate = await GetAsync(session.Id, cancellationToken);
        toUpdate?.Change(session);
    }

    public async Task<Models.Session?> GetAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        return await _context.Sessions.FirstOrDefaultAsync(c => c.Id == sessionId, cancellationToken);
    }

    public async Task<List<Models.Session>> GetSessionsByClientIdAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return await _context.Sessions
            .Where(session => session.ClientId == clientId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Models.Session>> GetSessionsByPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await _context.Sessions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await GetAsync(sessionId, cancellationToken);
        if (session is not null)
            Delete(session);
    }

    public void Delete(Models.Session session)
    {
        _context.Sessions.Remove(session);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}