using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(i => i.Price)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(i => i.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(i => i.OrderId);

            builder.HasOne(i => i.ProductVariant)
                   .WithMany()
                   .HasForeignKey(i => i.ProductVariantId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
