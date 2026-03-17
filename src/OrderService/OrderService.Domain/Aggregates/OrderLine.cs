using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;
using ProdId = OrderService.Domain.ValueObjects.ProductId;

namespace OrderService.Domain.Aggregates;

public sealed class OrderLine : Entity<Guid>
{
    public ProdId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money Price { get; private set; }

    private OrderLine(
        Guid id,
        ProdId productId,
        string productName,
        Quantity quantity,
        Money price) : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
    }

    private OrderLine() : base(Guid.Empty) //EF Core
    {
        ProductId = null!;
        ProductName = null!;
        Quantity = null!;
        Price = null!;
    }

    public static OrderLine Create(
        ProdId productId,
        string productName,
        Quantity quantity,
        Money price)
    {
        if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("ApplicationName is required.", nameof(productName));

        return new(Guid.NewGuid(), productId, productName, quantity, price);
    }
    internal void IncreaseQuantity(Quantity amount) => Quantity = Quantity.Add(amount);
    internal void ChangeQuantity(Quantity newQuantity) => Quantity = newQuantity;
}
