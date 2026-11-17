
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        builder.ToTable("phone_numbers");

        // Clave primaria
        builder.HasKey(p => p.PhoneId);

        builder.Property(p => p.PhoneId)
            .HasColumnName("phone_id");

        // Número telefónico
        builder.Property(p => p.Number)
            .HasColumnName("number")
            .HasMaxLength(8)
            .IsRequired();

        // Evitar números telefónicos duplicados
        builder.HasIndex(p => p.Number)
            .IsUnique();
    }
}
