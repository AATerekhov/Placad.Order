using OrderService.Domain.Abstractions;

namespace OrderService.Domain.ValueObjects;

public sealed class CouponCode : ValueObject
{
    public string Value { get; }

    private CouponCode(string value) => Value = value;

    public static CouponCode From(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("CouponCode cannot be empty.", nameof(value));
        return new(value.ToUpperInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value;
}
