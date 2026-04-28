namespace _8Boys.DTOs
{
    public class OrderDetailsDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public decimal ShippingPrice { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerProfile { get; set; }

        public IEnumerable<OrderItemDTO> Items { get; set; }
    }

    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string VariantCode { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Thumbnail { get; set; }
    }

    public class OrderSummaryDTO
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}