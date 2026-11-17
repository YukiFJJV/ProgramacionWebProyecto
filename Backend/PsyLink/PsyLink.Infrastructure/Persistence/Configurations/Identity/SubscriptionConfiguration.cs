
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Identity;

namespace PsyLink.Infrastructure.Persistence.Configurations.Identity;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(s => s.SubscriptionId);

        builder.Property(s => s.SubscriptionId)
            .HasColumnName("subscription_id");

        builder.Property(s => s.SubplanId)
            .HasColumnName("subplan_id")
            .IsRequired();

        builder.Property(s => s.TenantId)
            .HasColumnName("tenant_id")
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(s => s.StartsAt)
            .HasColumnName("starts_at")
            .IsRequired();

        builder.Property(s => s.EndsAt)
            .HasColumnName("ends_at");

        builder.HasOne(s => s.Subplan)
            .WithMany()
            .HasForeignKey(s => s.SubplanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Tenant)
            .WithMany()
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.SubplanId);
    }
}
