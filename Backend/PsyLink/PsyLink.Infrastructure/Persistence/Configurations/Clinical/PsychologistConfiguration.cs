
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Clinical;

namespace PsyLink.Infrastructure.Persistence.Configurations.Clinical;

public class PsychologistConfiguration : IEntityTypeConfiguration<Psychologist>
{
    public void Configure(EntityTypeBuilder<Psychologist> builder)
    {
        builder.ToTable("psychologists");

        builder.HasKey(p => p.PsychologistId);

        builder.Property(p => p.PsychologistId)
            .HasColumnName("psychologist_id");

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(p => p.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(p => p.SpecialtyId)
            .HasColumnName("specialty_id")
            .IsRequired();

        builder.Property(p => p.AdminId)
            .HasColumnName("admin_id");

        builder.Property(p => p.LicenseNumber)
            .HasColumnName("license_number")
            .HasColumnType("char(6)")
            .IsRequired();

        builder.Property(p => p.Biography)
            .HasColumnName("biography");

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Tenant)
            .WithMany()
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Specialty)
            .WithMany()
            .HasForeignKey(p => p.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Admin)
            .WithMany()
            .HasForeignKey(p => p.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasIndex(p => p.LicenseNumber).IsUnique();
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.SpecialtyId);
    }
}
