using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models.InventoryLogs
{
    public class InventoryLogConfiguration
    : IEntityTypeConfiguration<InventoryLog>
    {
        public void Configure(EntityTypeBuilder<InventoryLog> builder)
        {
            builder.Property(i => i.Reason)
                   .HasMaxLength(200);

            builder.Property(i => i.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(i => i.ProductVariant)
                   .WithMany()
                   .HasForeignKey(i => i.ProductVariantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
