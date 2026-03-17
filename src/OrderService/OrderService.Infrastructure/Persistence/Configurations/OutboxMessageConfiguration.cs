using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Infrastructure.Persistence.Outbox;

namespace OrderService.Infrastructure.Persistence.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");

        builder.Property(m => m.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(m => m.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(m => m.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.Property(m => m.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(m => m.Error)
            .HasColumnName("error");

        // Partial index for efficient polling — only unprocessed rows
        builder.HasIndex(m => m.ProcessedAt)
            .HasFilter("processed_at IS NULL")
            .HasDatabaseName("ix_outbox_messages_unprocessed");
    }
}
