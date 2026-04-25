using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> builder)
        {
            builder.Property(s => s.TrackingNumber)
                   .HasMaxLength(100);

            builder.Property(s => s.Carrier)
                   .HasMaxLength(100);

            builder.Property(s => s.Status)
                   .HasMaxLength(50);

            builder.HasOne(s => s.Order)
                   .WithOne()
                   .HasForeignKey<Shipping>(s => s.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
