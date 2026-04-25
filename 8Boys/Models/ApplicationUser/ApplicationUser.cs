using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace _8Boys.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }


        // Navigation
        public ICollection<Address> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}
