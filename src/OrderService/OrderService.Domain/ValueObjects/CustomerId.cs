using OrderService.Domain.Abstractions;

namespace OrderService.Domain.ValueObjects;

public sealed class CustomerId : ValueObject
{
    public Guid Value { get; }

    private CustomerId(Guid value) => Value = value;

    public static CustomerId From(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("CustomerId cannot be empty.", nameof(value));
        return new(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value.ToString();
}
