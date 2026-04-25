using Microsoft.AspNetCore.Http;

namespace _8Boys.DTOs
{
    public class AddProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }

        public string Code { get; set; }
        public int? ColorId { get; set; }
        public string Size { get; set; }
        public decimal RealPrice { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int StockQuantity { get; set; }
        public int? BadgeId { get; set; }
        public string? HexCode { get; set; }
        public string? ColorName { get; set; }

        // Accept uploaded files when creating product (multipart/form-data)
        public IEnumerable<IFormFile> ImageFiles { get; set; }
    }
}
