using _8Boys.Context;
using _8Boys.Models;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Repositry
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(_8BoysContext db) : base(db) { }

        public async Task<Cart?> GetUserCartAsync(string userId)
        {
            return await _db.Carts.
                Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
