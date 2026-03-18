namespace Orders.Application.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    IReadOnlyList<OrderLineDto> OrderLines,
    decimal TotalAmount,
    string Currency,
    string? PaymentReference,
    DateTime CreatedAt);

public record OrderLineDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    string Currency);
