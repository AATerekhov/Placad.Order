namespace OrderService.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string OrderType,
    string Status,
    BillingAddressDto BillingAddress,
    IReadOnlyList<OrderLineDto> OrderLines,
    string? CouponCode,
    decimal TotalAmount,
    string Currency,
    string? PaymentReference,
    DateTime CreatedAt);

public record BillingAddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country);

public record OrderLineDto(
    Guid Id,
    Guid ApplicationId,
    string ApplicationName,
    Guid PlanId,
    string PlanName,
    string BillingCycle,
    decimal Price,
    string Currency);
