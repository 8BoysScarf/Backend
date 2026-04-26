namespace _8Boys.DTOs
{
    public class ProductCardDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string ThumbnailUrl { get; set; }
        public IEnumerable<string> Badges { get; set; }
        public string Size { get; set; }
        public decimal? Discount { get; set; }
        public string HexCode { get; set; }
        public string Code { get; set; }
        public int StockQuantity { get; set; }
    }
}