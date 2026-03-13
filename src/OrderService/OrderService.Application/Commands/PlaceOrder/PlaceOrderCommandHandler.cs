using MediatR;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Enums;
using OrderService.Domain.Repositories;
using OrderService.Domain.ValueObjects;
using AppId = OrderService.Domain.ValueObjects.ApplicationId;

namespace OrderService.Application.Commands.PlaceOrder;

internal sealed class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
{
    private readonly IOrderRepository _repository;

    public PlaceOrderCommandHandler(IOrderRepository repository) => _repository = repository;

    public async Task<Guid> Handle(PlaceOrderCommand cmd, CancellationToken ct)
    {
        var customerId = CustomerId.From(cmd.CustomerId);
        var billingAddress = BillingAddress.Of(
            cmd.BillingAddress.Street,
            cmd.BillingAddress.City,
            cmd.BillingAddress.PostalCode,
            cmd.BillingAddress.Country);

        var orderLines = cmd.OrderLines.Select(l => OrderLine.Create(
            AppId.From(l.ApplicationId),
            l.ApplicationName,
            PlanId.From(l.PlanId),
            l.PlanName,
            l.BillingCycle,
            Money.Of(l.Price, l.Currency)));

        CouponCode? coupon = cmd.CouponCode is not null ? CouponCode.From(cmd.CouponCode) : null;

        var order = Order.Place(customerId, cmd.OrderType, billingAddress, orderLines, coupon);

        await _repository.AddAsync(order, ct);

        return order.Id.Value;
    }
}
