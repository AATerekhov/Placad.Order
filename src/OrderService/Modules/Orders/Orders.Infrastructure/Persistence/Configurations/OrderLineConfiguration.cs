using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Aggregates;
using Orders.Domain.ValueObjects;
using ProdId = Orders.Domain.ValueObjects.ProductId;

namespace Orders.Infrastructure.Persistence.Configurations;

internal sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("order_lines");

        builder.HasKey(ol => ol.Id);
        builder.Property(ol => ol.Id)
            .HasColumnName("id");

        builder.Property(ol => ol.ProductId)
            .HasConversion(id => id.Value, value => ProdId.From(value))
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(ol => ol.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ol => ol.Quantity)
            .HasConversion(q => q.Value, v => Quantity.Of(v))
            .HasColumnName("quantity")
            .IsRequired();

        builder.OwnsOne(ol => ol.Price, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("price_amount")
                .HasPrecision(18, 4)
                .IsRequired();

            price.Property(m => m.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}
