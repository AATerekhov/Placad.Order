using OrderService.Domain.Abstractions;

namespace OrderService.Domain.ValueObjects;

public sealed class PlanId : ValueObject
{
    public Guid Value { get; }

    private PlanId(Guid value) => Value = value;

    public static PlanId From(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("PlanId cannot be empty.", nameof(value));
        return new(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents() { yield return Value; }

    public override string ToString() => Value.ToString();
}
