namespace _8Boys.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsDefault { get; set; }

        public string City { get; set; }
        public string Street { get; set; }
    }
}
