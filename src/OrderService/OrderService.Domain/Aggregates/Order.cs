using OrderService.Domain.Abstractions;
using OrderService.Domain.Enums;
using OrderService.Domain.Events;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Aggregates;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderLine> _orderLines = [];

    public CustomerId CustomerId { get; private set; }
    public OrderType OrderType { get; private set; }
    public OrderStatus Status { get; private set; }
    public BillingAddress BillingAddress { get; private set; }
    public IReadOnlyList<OrderLine> OrderLines => _orderLines.AsReadOnly();
    public CouponCode? CouponCode { get; private set; }
    public Money TotalAmount { get; private set; }
    public PaymentReference? PaymentReference { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order(
        OrderId id,
        CustomerId customerId,
        OrderType orderType,
        BillingAddress billingAddress,
        CouponCode? couponCode) : base(id)
    {
        CustomerId = customerId;
        OrderType = orderType;
        Status = OrderStatus.Pending;
        BillingAddress = billingAddress;
        CouponCode = couponCode;
        TotalAmount = Money.Zero("USD");
        CreatedAt = DateTime.UtcNow;
    }

    public static Order Place(
        CustomerId customerId,
        OrderType orderType,
        BillingAddress billingAddress,
        IEnumerable<OrderLine> orderLines,
        CouponCode? couponCode = null)
    {
        var order = new Order(OrderId.New(), customerId, orderType, billingAddress, couponCode);

        foreach (var line in orderLines)
            order.AddOrderLine(line);

        order.RaiseDomainEvent(new OrderPlaced(order.Id, customerId, order.CreatedAt));
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
