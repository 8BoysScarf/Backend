namespace _8Boys.DTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string UserId { get; set; }

        // Added: include roles in the authentication response
        public IList<string> Roles { get; set; } = new List<string>();
    }
}