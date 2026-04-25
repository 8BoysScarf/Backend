using Microsoft.AspNetCore.Http;

namespace _8Boys.DTOs
{
    public class AddVariantDTO
    {
        public int ProductId { get; set; }
        public int? ColorId { get; set; }
        public string? ColorName { get; set; }
        public string? HexCode { get; set; }
        public string Size { get; set; }
        public decimal RealPrice { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int StockQuantity { get; set; }
        public string Code { get; set; }
        public IEnumerable<IFormFile> ImageFiles { get; set; }
    }
}