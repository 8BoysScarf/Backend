using _8Boys.Models;

namespace _8Boys.Repositry
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<double> GetAverageRatingAsync(int productId);
    }
}
