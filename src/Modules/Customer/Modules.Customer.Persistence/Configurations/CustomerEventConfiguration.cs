using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Customer.Persistence.Entities;

namespace Modules.Customer.Persistence.Configurations;

public sealed class CustomerEventConfiguration : IEntityTypeConfiguration<CustomerEvent>
{
    public void Configure(EntityTypeBuilder<CustomerEvent> builder)
    {
        builder.ToTable("customer_events");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Type).IsRequired();
        builder.Property(e => e.Data).IsRequired();
        builder.Property(e => e.Version).IsRequired();
        builder.Property(e => e.OccurredOnUtc).IsRequired();
        builder.HasIndex(e => new { e.CustomerId, e.Version }).IsUnique();
    }
}

