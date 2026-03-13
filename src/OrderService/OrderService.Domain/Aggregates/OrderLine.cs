using OrderService.Domain.Abstractions;
using OrderService.Domain.ValueObjects;
using AppId = OrderService.Domain.ValueObjects.ApplicationId;

namespace OrderService.Domain.Aggregates;

public sealed class OrderLine : Entity<Guid>
{
    public AppId ApplicationId { get; private set; }
    public string ApplicationName { get; private set; }
    public PlanId PlanId { get; private set; }
    public string PlanName { get; private set; }
    public string BillingCycle { get; private set; }
    public Money Price { get; private set; }

    private OrderLine(
        Guid id,
        AppId applicationId,
        string applicationName,
        PlanId planId,
        string planName,
        string billingCycle,
        Money price) : base(id)
    {
        ApplicationId = applicationId;
        ApplicationName = applicationName;
        PlanId = planId;
        PlanName = planName;
        BillingCycle = billingCycle;
        Price = price;
    }

    public static OrderLine Create(
        AppId applicationId,
        string applicationName,
        PlanId planId,
        string planName,
        string billingCycle,
        Money price)
    {
        if (string.IsNullOrWhiteSpace(applicationName)) throw new ArgumentException("ApplicationName is required.", nameof(applicationName));
        if (string.IsNullOrWhiteSpace(planName)) throw new ArgumentException("PlanName is required.", nameof(planName));
        if (string.IsNullOrWhiteSpace(billingCycle)) throw new ArgumentException("BillingCycle is required.", nameof(billingCycle));

        return new(Guid.NewGuid(), applicationId, applicationName, planId, planName, billingCycle, price);
    }
}
