using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Aggregates;
using Orders.Domain.ValueObjects;

namespace Orders.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(id => id.Value, value => OrderId.From(value))
            .HasColumnName("id");

        builder.Property(o => o.CustomerId)
            .HasConversion(id => id.Value, value => CustomerId.From(value))
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(o => o.PaymentReference)
            .HasConversion(
                pr => pr == null ? null : pr.Value,
                value => value == null ? null : PaymentReference.From(value))
            .HasColumnName("payment_reference");

        // TotalAmount is computed from OrderLines — not stored
        builder.Ignore(o => o.TotalAmount);
        builder.Ignore(o => o.DomainEvents);

        // OrderLines collection via backing field
        builder.HasMany(o => o.OrderLines)
            .WithOne()
            .HasForeignKey("order_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.OrderLines)
            .HasField("_orderLines")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(o => o.RowVersion)
            .IsRowVersion();
    }
}
