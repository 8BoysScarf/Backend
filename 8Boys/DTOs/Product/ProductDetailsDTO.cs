namespace _8Boys.DTOs
{
    public class ProductDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<ProductVariantDTO> Variants { get; set; }
        public IEnumerable<string> Badges { get; set; }
    }

    public class ProductVariantDTO
    {
        public int Id { get; set; }
        public int? ColorId { get; set; }
        public string ColorName { get; set; }
        public string ColorHex { get; set; }
        public string Size { get; set; }
        public decimal RealPrice { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int StockQuantity { get; set; }
        public string Code { get; set; }
        public IEnumerable<string> ImageUrls { get; set; }
    }
}