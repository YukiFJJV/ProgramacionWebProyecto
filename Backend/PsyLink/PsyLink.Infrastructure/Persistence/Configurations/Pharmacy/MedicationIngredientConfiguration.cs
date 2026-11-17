
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsyLink.Domain.Pharmacy;

namespace PsyLink.Infrastructure.Persistence.Configurations.Pharmacy;

public class MedicationIngredientConfiguration
    : IEntityTypeConfiguration<MedicationIngredient>
{
    public void Configure(EntityTypeBuilder<MedicationIngredient> builder)
    {
        builder.ToTable("medication_ingredients");

        builder.HasKey(mi => new
        {
            mi.MedicationId,
            mi.IngredientId
        });

        builder.Property(mi => mi.MedicationId)
            .HasColumnName("medication_id");

        builder.Property(mi => mi.IngredientId)
            .HasColumnName("ingredient_id");

        builder.HasOne(mi => mi.Medication)
            .WithMany(m => m.Ingredients)
            .HasForeignKey(mi => mi.MedicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mi => mi.Ingredient)
            .WithMany()
            .HasForeignKey(mi => mi.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(mi => mi.IngredientId);
    }
}
