namespace _8Boys.DTOs
{
    public class UpdateAddressDTO
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; }
    }
}