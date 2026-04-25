namespace _8Boys.Models
{
    public class ProductBadge
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int BadgeId { get; set; }
        public Badge Badge { get; set; }
    }
}
