
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Billing;

namespace PsyLink.Infrastructure.Persistence.Configurations.Billing;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.ToTable("payment_methods");

        builder.HasKey(p => p.MethodId);

        builder.Property(p => p.MethodId)
            .HasColumnName("method_id");

        builder.Property(p => p.Method)
            .HasColumnName("method")
            .IsRequired();
    }
}
