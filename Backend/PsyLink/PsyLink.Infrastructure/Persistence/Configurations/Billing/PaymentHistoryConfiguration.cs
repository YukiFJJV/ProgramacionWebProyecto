
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Billing;

namespace PsyLink.Infrastructure.Persistence.Configurations.Billing;

public class PaymentHistoryConfiguration
    : IEntityTypeConfiguration<PaymentHistory>
{
    public void Configure(EntityTypeBuilder<PaymentHistory> builder)
    {
        builder.ToTable("payment_history");

        builder.HasKey(p => p.PaymentId);

        builder.Property(p => p.PaymentId)
            .HasColumnName("payment_id");

        builder.Property(p => p.MethodId)
            .HasColumnName("method_id")
            .IsRequired();

        builder.Property(p => p.FiscalAuthorizationId)
            .HasColumnName("fiscal_authorization_id")
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(200);

        builder.Property(p => p.IssueDate)
            .HasColumnName("issue_date")
            .IsRequired();

        builder.Property(p => p.BillNumber)
            .HasColumnName("bill_number")
            .HasColumnType("char(16)");

        builder.Property(p => p.Discount)
            .HasColumnName("discount")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.Total)
            .HasColumnName("total")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasOne(p => p.Method)
            .WithMany()
            .HasForeignKey(p => p.MethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.FiscalAuthorization)
            .WithMany()
            .HasForeignKey(p => p.FiscalAuthorizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.MethodId);
        builder.HasIndex(p => p.FiscalAuthorizationId);
        builder.HasIndex(p => p.IssueDate);
    }
}
