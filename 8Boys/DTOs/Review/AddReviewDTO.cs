namespace _8Boys.DTOs
{
    public class AddReviewDTO
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}