using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class ProductBadgeConfiguration
    : IEntityTypeConfiguration<ProductBadge>
    {
        public void Configure(EntityTypeBuilder<ProductBadge> builder)
        {
            builder.HasKey(pb => new { pb.ProductId, pb.BadgeId });

            builder.HasOne(pb => pb.Product)
                   .WithMany(p => p.ProductBadges)
                   .HasForeignKey(pb => pb.ProductId);

            builder.HasOne(pb => pb.Badge)
                   .WithMany(b => b.ProductBadges)
                   .HasForeignKey(pb => pb.BadgeId);
        }
    }
}
