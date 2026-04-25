namespace _8Boys.Models
{
    public class Badge
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ProductBadge> ProductBadges { get; set; }
    }
}
