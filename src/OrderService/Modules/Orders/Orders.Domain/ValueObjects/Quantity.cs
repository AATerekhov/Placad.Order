using SharedKernel;

namespace Orders.Domain.ValueObjects;

public sealed class Quantity : ValueObject
{
    public int Value { get; private set; }

    private Quantity() { } // EF Core

    private Quantity(int value) => Value = value;

    public static Quantity Of(int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be greater than zero.");
        return new(value);
    }

    public Quantity Add(Quantity other) => Of(Value + other.Value);

    public Quantity Subtract(Quantity other)
    {
        if (other.Value >= Value)
            throw new InvalidOperationException("Cannot subtract: result would be zero or negative.");
        return Of(Value - other.Value);
    }

    public bool IsGreaterThan(Quantity other) => Value > other.Value;

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value.ToString();
}
