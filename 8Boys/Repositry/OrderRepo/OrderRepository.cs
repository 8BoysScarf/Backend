using _8Boys.Context;
using _8Boys.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace _8Boys.Repositry
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(_8BoysContext db) : base(db) { }

        public async Task<Order?> GetOrderWithItemsAsync(int id)
        {
            return await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _db.Orders
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }
}
