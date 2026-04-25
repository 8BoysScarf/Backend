namespace _8Boys.Models
{
    public class InventoryLog
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }

        public int ChangeQuantity { get; set; } // + or -
        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
