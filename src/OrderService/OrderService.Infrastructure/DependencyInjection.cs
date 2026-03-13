using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.ExternalServices;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Persistence
        services.AddSingleton<InMemoryOutboxStore>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // External services
        services.AddSingleton<CatalogServiceClient>();

        // Outbox background processor
        services.AddHostedService<OutboxProcessor>();

        // MassTransit with in-memory transport
        services.AddMassTransit(cfg =>
        {
            cfg.AddOrderServiceConsumers();

            cfg.UsingInMemory((ctx, busCfg) =>
            {
                busCfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
