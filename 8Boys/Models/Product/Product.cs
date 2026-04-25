namespace _8Boys.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<ProductVariant> Variants { get; set; }
        public ICollection<ProductBadge> ProductBadges { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
