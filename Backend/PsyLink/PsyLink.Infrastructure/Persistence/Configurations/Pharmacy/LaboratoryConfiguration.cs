
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Pharmacy;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Pharmacy;

public class LaboratoryConfiguration : IEntityTypeConfiguration<Laboratory>
{
    public void Configure(EntityTypeBuilder<Laboratory> builder)
    {
        builder.ToTable("laboratories");

        builder.HasKey(l => l.LaboratoryId);

        builder.Property(l => l.LaboratoryId)
            .HasColumnName("laboratory_id");

        builder.Property(l => l.PhoneId)
            .HasColumnName("phone_id");

        builder.Property(l => l.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.HasOne(l => l.Phone)
            .WithMany()
            .HasForeignKey(l => l.PhoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(l => l.PhoneId);
    }
}
