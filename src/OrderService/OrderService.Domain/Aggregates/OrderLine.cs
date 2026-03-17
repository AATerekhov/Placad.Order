using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;
using ProdId = OrderService.Domain.ValueObjects.ProductId;

namespace OrderService.Domain.Aggregates;

public sealed class OrderLine : Entity<Guid>
{
    public ProdId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }

    private OrderLine(
        Guid id,
        ProdId productId,
        string productName,
        int quantity,
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
        Quantity = 0;
        Price = null!;
    }

    public static OrderLine Create(
        ProdId productId,
        string productName,
        int quantity,
        Money price)
    {
        if (string.IsNullOrWhiteSpace(productName)) throw new ArgumentException("ApplicationName is required.", nameof(productName));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity),"Quantity must be greater than zero.");

        return new(Guid.NewGuid(), productId, productName, quantity, price);
    }
    internal void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        Quantity += quantity;
    }

    internal void ChangeQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(newQuantity), "Quantity must be greater than zero.");

        Quantity = newQuantity;
    }
}
