using OrderService.Domain.Abstractions;
using OrderService.Domain.Enums;
using OrderService.Domain.Events;
using OrderService.Domain.ValueObjects;
using ProdId = OrderService.Domain.ValueObjects.ProductId;

namespace OrderService.Domain.Aggregates;

public sealed class Order : AggregateRoot<OrderId>
{
    private List<OrderLine> _orderLines = [];

    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines;
    public Money TotalAmount => _orderLines.Count == 0
        ? Money.Zero("USD")
        : Money.Of(_orderLines.Sum(l => l.Price.Amount), _orderLines[0].Price.Currency);
    public PaymentReference? PaymentReference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = default!;

    private Order() : base(OrderId.New()) { CustomerId = null!; } // EF Core

    private Order(
        OrderId id,
        CustomerId customerId) : base(id)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Order Place(
        CustomerId customerId,
        IEnumerable<OrderLineData> lines)
    {
        var order = new Order(OrderId.New(), customerId);

        foreach (var l in lines)
            order.AddOrderLine(OrderLine.Create(
                ProdId.From(l.ProductId),
                l.ProductName,
                l.Quantity,
                Money.Of(l.Amount, l.Currency)));

        order.RaiseDomainEvent(new OrderPlacedDomainEvent(order.Id, customerId, order.CreatedAt));
        return order;
    }

    private void AddOrderLine(OrderLine line) => _orderLines.Add(line);

    public void Confirm(PaymentReference paymentReference)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm an order in '{Status}' status.");

        if (Status == OrderStatus.Pending && _orderLines.Count == 0)
            throw new InvalidOperationException("Paid order cannot be empty.");

        Status = OrderStatus.Confirmed;
        PaymentReference = paymentReference;
        RaiseDomainEvent(new OrderConfirmedDomainEvent(Id, paymentReference, DateTime.UtcNow));
    }

    public void Fail(string reason)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot fail an order in '{Status}' status.");

        Status = OrderStatus.Failed;
        RaiseDomainEvent(new OrderFailedDomainEvent(Id, reason, DateTime.UtcNow));
    }

    public void Refund()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be refunded.");

        Status = OrderStatus.Failed;
        RaiseDomainEvent(new OrderRefundedDomainEvent(Id, TotalAmount, DateTime.UtcNow));
    }
}
