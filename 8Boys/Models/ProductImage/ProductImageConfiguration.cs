using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class ProductImageConfiguration
    : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.Property(i => i.ImageUrl)
                   .IsRequired();

            builder.HasOne(i => i.ProductVariant)
                   .WithMany(v => v.Images)
                   .HasForeignKey(i => i.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
