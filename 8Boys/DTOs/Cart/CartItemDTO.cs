namespace _8Boys.DTOs
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }

        // Variant details
        public string Code { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }

        // Product details
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string? ColorName { get; set; }
        public string? ColorHex { get; set; }
        public decimal? Discount { get; set; }
        public decimal RealPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}