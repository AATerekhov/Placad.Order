using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Domain.Repositories;
using Orders.Infrastructure.ExternalServices;
using Orders.Infrastructure.Messaging;
using Orders.Infrastructure.Persistence;
using Orders.Infrastructure.Persistence.Interceptors;
using Orders.Infrastructure.Persistence.Outbox.Abstractions;
using Orders.Infrastructure.Persistence.Outbox.Implementations;
using Orders.Infrastructure.Persistence.Repositories;

namespace Orders.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core + PostgreSQL + Interceptor
        services.AddSingleton<ConvertDomainEventsToOutboxMessagesInterceptor>();
        services.AddDbContext<OrderDbContext>((sp, options) =>
           {
               var interceptor = sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>();

               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
               options.AddInterceptors(interceptor);
           });

        // Persistence
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<OrderDbContext>());

        // External services
        services.AddSingleton<CatalogServiceClient>();

        // Outbox background processor
        services.AddScoped<IIntegrationEventPublisher, LoggingIntegrationEventPublisher>();
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
