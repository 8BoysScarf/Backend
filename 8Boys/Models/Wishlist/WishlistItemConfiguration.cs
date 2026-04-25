using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class WishlistItemConfiguration
    : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasOne(i => i.Product)
                   .WithMany()
                   .HasForeignKey(i => i.ProductId);

            builder.HasIndex(i => new { i.WishlistId, i.ProductId })
                   .IsUnique(); // prevent duplicates
        }
    }
}
