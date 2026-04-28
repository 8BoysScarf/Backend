using _8Boys.Context;
using _8Boys.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace _8Boys.Repositry
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(_8BoysContext db) : base(db) { }

        public async Task<Product?> GetWithDetailsAsync(int id)
        {
            return await _db.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.ProductBadges)
                    .ThenInclude(pb => pb.Badge)
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _db.Products
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string keyword)
        {
            return await _db.Products
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> FilterAsync(decimal? min, decimal? max)
        {
            var query = _db.Products
                .Include(p => p.Variants)
                .AsQueryable();

            if (min.HasValue || max.HasValue)
            {
                query = query.Where(p =>
                    p.Variants.Any(v =>
                        (!min.HasValue || v.Price >= min) &&
                        (!max.HasValue || v.Price <= max)
                    ));
            }

            return await query.ToListAsync();
        }
    }
}
