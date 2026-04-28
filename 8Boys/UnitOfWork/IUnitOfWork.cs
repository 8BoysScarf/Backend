using _8Boys.Models;
using _8Boys.Repositry;

namespace _8Boys
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Color> Colors { get; }
        IGenericRepository<Badge> Badges { get; }
        IGenericRepository<ProductVariant> ProductVariants { get; }
        ICartRepository Carts { get; }
        IGenericRepository<CartItem> CartItems { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<Wishlist> Wishlists { get; }
        IGenericRepository<WishlistItem> WishlistItems { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Shipping> Shippings { get; }
        IGenericRepository<CityShipping> CityShippings { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
