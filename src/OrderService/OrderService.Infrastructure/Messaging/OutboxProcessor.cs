using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure.Messaging;

public sealed class OutboxProcessor : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    private readonly InMemoryOutboxStore _outbox;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        InMemoryOutboxStore outbox,
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _outbox = outbox;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxAsync(stoppingToken);
            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task ProcessOutboxAsync(CancellationToken ct)
    {
        var messages = _outbox.GetUnprocessed();
        if (messages.Count == 0) return;

        await using var scope = _scopeFactory.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();

        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.EventType);
                if (eventType is null)
                {
                    _logger.LogWarning("Unknown event type: {EventType}", message.EventType);
                    _outbox.MarkProcessed(message.Id);
                    continue;
                }

                var payload = JsonSerializer.Deserialize(message.Payload, eventType);
                if (payload is not null)
                    await bus.Publish(payload, eventType, ct);

                _outbox.MarkProcessed(message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
            }
        }
    }
}
