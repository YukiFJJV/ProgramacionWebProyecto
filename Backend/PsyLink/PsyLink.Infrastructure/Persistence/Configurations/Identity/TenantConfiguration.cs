
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");

        // Clave primaria
        builder.HasKey(t => t.TenantId);

        builder.Property(t => t.TenantId)
            .HasColumnName("tenant_id");

        // Nombre comercial
        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        // Estado
        builder.Property(t => t.Active)
            .HasColumnName("active")
            .IsRequired();

        // Datos opcionales
        builder.Property(t => t.PhoneId)
            .HasColumnName("phone_id");

        builder.Property(t => t.Email)
            .HasColumnName("email");

        builder.Property(t => t.Rtn)
            .HasColumnName("rtn");

        builder.Property(t => t.LinkToken)
            .HasColumnName("link_token");

        // Restricciones de unicidad
        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.HasIndex(t => t.Rtn)
            .IsUnique();
    }
}
