using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace UserService.Data.Repositories.Outbox;

public class OutboxRepository : IOutboxRepository
{
    private readonly UsersDbContext _context;

    public OutboxRepository(UsersDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken)
    {
        return await _context.OutboxMessages
            .Where(x => x.ProcessedAt == null)
            .ToListAsync(cancellationToken);
    }

    public async Task AddContentMessageAsync<T>(T message,
        string routingKey,
        CancellationToken cancellationToken)
        where T : notnull
    {
        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = message.GetType().FullName!,
            Content = JsonSerializer.Serialize(message),
            RoutingKey = routingKey,
            OccurredAt = DateTime.Now
        };
        
        await _context.OutboxMessages.AddAsync(outboxMessage, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }
}