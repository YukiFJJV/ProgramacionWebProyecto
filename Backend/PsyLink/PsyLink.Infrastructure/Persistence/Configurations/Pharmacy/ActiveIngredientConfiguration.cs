
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Infrastructure.Persistence.Configurations.Pharmacy;

public class ActiveIngredientConfiguration : IEntityTypeConfiguration<ActiveIngredient>
{
    public void Configure(EntityTypeBuilder<ActiveIngredient> builder)
    {
        builder.ToTable("active_ingredients");

        builder.HasKey(a => a.IngredientId);

        builder.Property(a => a.IngredientId)
            .HasColumnName("ingredient_id");

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .IsRequired();
    }
}
