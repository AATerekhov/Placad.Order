using OrderService.Domain.Abstractions;

namespace OrderService.Domain.ValueObjects;

public sealed class ProductId : ValueObject
{
    public Guid Value { get; }

    private ProductId(Guid value) => Value = value;

    public static ProductId From(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("ApplicationId cannot be empty.", nameof(value));
        return new(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value.ToString();
}
