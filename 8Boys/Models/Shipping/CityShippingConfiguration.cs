using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _8Boys.Models
{
    public class CityShippingConfiguration : IEntityTypeConfiguration<CityShipping>
    {
        public void Configure(EntityTypeBuilder<CityShipping> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.City).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Price).HasColumnType("decimal(18,2)").IsRequired();
            builder.HasIndex(c => c.City).IsUnique();
        }
    }
}
