namespace _8Boys.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        // Changed: image now belongs to a specific product variant instead of the product
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
