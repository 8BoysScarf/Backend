using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class WishlistConfiguration
    : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasOne(w => w.User)
                   .WithOne(u => u.Wishlist)
                   .HasForeignKey<Wishlist>(w => w.UserId);

            builder.HasMany(w => w.Items)
                   .WithOne(i => i.Wishlist)
                   .HasForeignKey(i => i.WishlistId);
        }
    }
}
