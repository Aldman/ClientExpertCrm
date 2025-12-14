using System.Text.Json;
using Shared.Messaging;
using UserService.Data.Repositories.Outbox;

namespace UserService.Outbox;

public class OutboxProcessor
{
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEventPublisher _eventPublisher;

    public OutboxProcessor(IOutboxRepository outboxRepository, IEventPublisher eventPublisher)
    {
        _outboxRepository = outboxRepository;
        _eventPublisher = eventPublisher;
    }
    
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var messages = await _outboxRepository.GetUnprocessedMessagesAsync(cancellationToken);
        foreach (var message in messages)
        {
            try
            {
                var messageType = Type.GetType(message.Type)!;
                var deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType)!;

                await _eventPublisher.PublishAsync(
                    message: deserializedMessage,
                    routingKey: message.RoutingKey,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception e)
            {
                message.Error = e.GetBaseException().Message;
            }
            finally
            {
                message.ProcessedAt = DateTime.Now;
            }
        }
        await _outboxRepository.SaveChangesAsync(cancellationToken);
    }
}