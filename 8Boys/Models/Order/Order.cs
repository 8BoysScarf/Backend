namespace _8Boys.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal ShippingPrice { get; set; }
        public OrderStatus Status { get; set; }

        public ICollection<OrderItem> Items { get; set; }
    }
}
