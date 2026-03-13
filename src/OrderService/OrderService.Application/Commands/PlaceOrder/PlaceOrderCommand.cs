using OrderService.Application.Abstractions;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands.PlaceOrder;

public record PlaceOrderCommand(
    Guid CustomerId,
    OrderType OrderType,
    PlaceOrderBillingAddress BillingAddress,
    IReadOnlyList<PlaceOrderLine> OrderLines,
    string? CouponCode) : ICommand<Guid>;

public record PlaceOrderBillingAddress(string Street, string City, string PostalCode, string Country);

public record PlaceOrderLine(
    Guid ApplicationId,
    string ApplicationName,
    Guid PlanId,
    string PlanName,
    string BillingCycle,
    decimal Price,
    string Currency);
