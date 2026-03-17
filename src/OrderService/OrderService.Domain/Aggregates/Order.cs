using OrderService.Domain.Abstractions;
using OrderService.Domain.Enums;
using OrderService.Domain.Events;
using OrderService.Domain.ValueObjects;
using ProdId = OrderService.Domain.ValueObjects.ProductId;

namespace OrderService.Domain.Aggregates;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderLine> _orderLines = [];

    public CustomerId CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public Money TotalAmount { get; private set; }
    public PaymentReference? PaymentReference { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() :base(OrderId.New())
    {
        CustomerId = null!;
        TotalAmount = null!;
    }

    private Order(
        OrderId id,
        CustomerId customerId) : base(id)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        TotalAmount = Money.Zero("USD");
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

    public void AddOrderLine(OrderLine line)
    {
        _orderLines.Add(line);
        TotalAmount = TotalAmount.Add(line.Price);
    }

    public void Confirm(PaymentReference paymentReference)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm an order in '{Status}' status.");

        Status = OrderStatus.Confirmed;
        PaymentReference = paymentReference;
        RaiseDomainEvent(new OrderConfirmed(Id, paymentReference, DateTime.UtcNow));
    }

    public void Fail(string reason)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot fail an order in '{Status}' status.");

        Status = OrderStatus.Failed;
        RaiseDomainEvent(new OrderFailed(Id, reason, DateTime.UtcNow));
    }

    public void Refund()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be refunded.");

        Status = OrderStatus.Failed;
        RaiseDomainEvent(new OrderRefunded(Id, TotalAmount, DateTime.UtcNow));
    }
}
