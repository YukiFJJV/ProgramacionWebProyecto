
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Billing;

namespace PsyLink.Infrastructure.Persistence.Configurations.Billing;

public class FiscalAuthorizationConfiguration
    : IEntityTypeConfiguration<FiscalAuthorization>
{
    public void Configure(EntityTypeBuilder<FiscalAuthorization> builder)
    {
        builder.ToTable("fiscal_authorizations");

        builder.HasKey(f => f.FiscalAuthorizationId);

        builder.Property(f => f.FiscalAuthorizationId)
            .HasColumnName("fiscal_authorization_id");

        builder.Property(f => f.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(f => f.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(f => f.Cai)
            .HasColumnName("cai")
            .HasColumnType("char(32)")
            .IsRequired();

        builder.Property(f => f.LimitDate)
            .HasColumnName("limit_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(f => f.InitialRange)
            .HasColumnName("initial_range")
            .HasColumnType("char(16)")
            .IsRequired();

        builder.Property(f => f.FinalRange)
            .HasColumnName("final_range")
            .HasColumnType("char(16)")
            .IsRequired();

        builder.HasOne(f => f.Patient)
            .WithMany()
            .HasForeignKey(f => f.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Tenant)
            .WithMany()
            .HasForeignKey(f => f.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => f.PatientId);
        builder.HasIndex(f => f.TenantId);
        builder.HasIndex(f => f.Cai).IsUnique();
    }
}
