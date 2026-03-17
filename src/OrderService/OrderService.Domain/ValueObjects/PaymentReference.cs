using OrderService.Domain.Abstractions;

namespace OrderService.Domain.ValueObjects;

public sealed class PaymentReference : ValueObject
{
    public string Value { get; }

    private PaymentReference(string value) => Value = value;

    public static PaymentReference From(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("PaymentReference cannot be empty.", nameof(value));
        return new(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value;
}
