using Shared.Models;

namespace UserService.Data.Repositories.Outbox;

public interface IOutboxRepository
{
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken);
    Task AddContentMessageAsync<T>(T message, string routingKey, CancellationToken cancellationToken) 
        where T : notnull;
    Task SaveChangesAsync(CancellationToken ct);
}