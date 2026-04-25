using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasOne(i => i.ProductVariant)
                   .WithMany()
                   .HasForeignKey(i => i.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => new { i.CartId, i.ProductVariantId })
                   .IsUnique(); // prevent duplicate item in cart
        }
    }
}
