
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        // Clave primaria
        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
            .HasColumnName("role_id");

        // Nombre del rol
        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        // Evitar roles duplicados
        builder.HasIndex(r => r.Name)
            .IsUnique();
    }
}
