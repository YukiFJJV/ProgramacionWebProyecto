
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        // Clave primaria
        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("user_id");

        // Datos de la cuenta
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .IsRequired();

        builder.Property(u => u.Active)
            .HasColumnName("active")
            .IsRequired();

        // Claves foráneas
        builder.Property(u => u.TenantId)
            .HasColumnName("tenant_id");

        builder.Property(u => u.PhoneId)
            .HasColumnName("phone_id");

        builder.Property(u => u.RoleId)
            .HasColumnName("role_id")
            .IsRequired();

        // Relación con Tenant (opcional)
        builder.HasOne(u => u.Tenant)
            .WithMany()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con PhoneNumber (opcional)
        builder.HasOne(u => u.Phone)
            .WithMany()
            .HasForeignKey(u => u.PhoneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con Role (obligatoria)
        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(u => new
        {
            u.TenantId,
            u.Email
        }).IsUnique();
    }
}
