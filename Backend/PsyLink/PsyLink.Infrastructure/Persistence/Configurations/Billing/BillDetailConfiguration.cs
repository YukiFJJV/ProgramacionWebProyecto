
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Billing;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Billing;

public class BillDetailConfiguration : IEntityTypeConfiguration<BillDetail>
{
    public void Configure(EntityTypeBuilder<BillDetail> builder)
    {
        builder.ToTable("bill_details");

        builder.HasKey(b => b.DetailId);

        builder.Property(b => b.DetailId)
            .HasColumnName("detail_id");

        builder.Property(b => b.PaymentId)
            .HasColumnName("payment_id")
            .IsRequired();

        builder.Property(b => b.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(b => b.Detail)
            .HasColumnName("detail")
            .IsRequired();

        builder.Property(b => b.CantHours)
            .HasColumnName("cant_hours")
            .IsRequired();

        builder.Property(b => b.Price)
            .HasColumnName("price")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(b => b.Subtotal)
            .HasColumnName("subtotal")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasOne(b => b.Payment)
            .WithMany(p => p.Details)
            .HasForeignKey(b => b.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.PaymentId);
        builder.HasIndex(b => b.TenantId);
    }
}
