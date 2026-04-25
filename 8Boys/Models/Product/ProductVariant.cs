using System.Drawing;

namespace _8Boys.Models
{
    public class ProductVariant
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int? ColorId { get; set; }
        public Color Color { get; set; }

        public string Size { get; set; }

        public decimal RealPrice { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }

        public int StockQuantity { get; set; }
        public string Code { get; set; }

        // Added: images belong to product variant
        public ICollection<ProductImage> Images { get; set; }
    }
}
