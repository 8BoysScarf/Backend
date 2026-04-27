using _8Boys.Models;
using _8Boys.Repositry;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Services
{
    public class WishlistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Wishlist> GetOrCreateWishlistAsync(string userId)
        {
            var wishlist = await _unitOfWork.Wishlists.Query().Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist != null) return wishlist;

            wishlist = new Wishlist { UserId = userId, Items = new List<WishlistItem>() };
            await _unitOfWork.Wishlists.AddAsync(wishlist);
            await _unitOfWork.SaveChangesAsync();
            return wishlist;
        }

        public async Task AddToWishlistAsync(string userId, int productId)
        {
            var wishlist = await GetOrCreateWishlistAsync(userId);
            if (wishlist.Items.Any(i => i.ProductId == productId)) return;

            var item = new WishlistItem { WishlistId = wishlist.Id, ProductId = productId };
            await _unitOfWork.WishlistItems.AddAsync(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlist = await _unitOfWork.Wishlists.Query().Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null) return;

            var item = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return;

            _unitOfWork.WishlistItems.Remove(item);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<int>> GetWishlistProductIdsAsync(string userId)
        {
            var wishlist = await _unitOfWork.Wishlists.Query().Include(w => w.Items).FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null) return Enumerable.Empty<int>();
            return wishlist.Items.Select(i => i.ProductId);
        }
    }
}
