using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Consumers;

namespace OrderService.Infrastructure.Messaging;

public static class MassTransitConfig
{
    public static IBusRegistrationConfigurator AddOrderServiceConsumers(
        this IBusRegistrationConfigurator cfg)
    {
        cfg.AddConsumer<PaymentCompletedConsumer>();
        cfg.AddConsumer<PaymentFailedConsumer>();
        return cfg;
    }
}
