using OrderService.Domain.Abstractions;

namespace OrderService.Domain.ValueObjects;

public sealed class ApplicationId : ValueObject
{
    public Guid Value { get; }

    private ApplicationId(Guid value) => Value = value;

    public static ApplicationId From(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("ApplicationId cannot be empty.", nameof(value));
        return new(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value.ToString();
}
