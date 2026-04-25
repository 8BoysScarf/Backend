using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class ProductVariantConfiguration: IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.Property(v => v.Price)
                   .HasColumnType("decimal(18,2)");
            builder.Property(v => v.RealPrice)
                   .HasColumnType("decimal(18,2)");
            builder.Property(v => v.Discount)
                    .HasColumnType("decimal(18,2)");

            builder.Property(v => v.Size)
                   .HasMaxLength(50);
            builder.HasIndex(v => v.Code).IsUnique();

            builder.HasOne(v => v.Product)
                   .WithMany(p => p.Variants)
                   .HasForeignKey(v => v.ProductId);

            builder.HasOne(v => v.Color)
                   .WithMany()
                   .HasForeignKey(v => v.ColorId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(v => new { v.ProductId, v.ColorId, v.Size })
                   .IsUnique(); // prevent duplicate variants
        }
    }
}
