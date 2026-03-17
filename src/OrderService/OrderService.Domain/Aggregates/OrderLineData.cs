namespace OrderService.Domain.Aggregates;

public record OrderLineData(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Amount,
    string Currency);
