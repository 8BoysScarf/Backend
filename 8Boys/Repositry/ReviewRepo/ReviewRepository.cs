using _8Boys.Context;
using _8Boys.Models;
using Microsoft.EntityFrameworkCore;

namespace _8Boys.Repositry
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(_8BoysContext db) : base(db) { }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            return await _db.Reviews
                .Where(r => r.ProductId == productId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
        }
    }
}
