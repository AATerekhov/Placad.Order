using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.ExternalServices;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Persistence.Interceptors;
using OrderService.Infrastructure.Persistence.Outbox.Abstractons;
using OrderService.Infrastructure.Persistence.Outbox.Implementations;
using OrderService.Infrastructure.Persistence.Repositories;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core + PostgreSQL + Interceptor
        services.AddDbContext<OrderDbContext>((sp, options) =>
           {
               var interceptor = sp.GetRequiredService<ConvertDomainEventsToOutboxMessagesInterceptor>();

               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")); 
               options.AddInterceptors(interceptor);
           });

        // Persistence
        services.AddScoped<IOrderRepository, OrderRepository>();

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
