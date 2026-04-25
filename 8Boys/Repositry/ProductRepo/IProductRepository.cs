using _8Boys.Models;

namespace _8Boys.Repositry
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetWithDetailsAsync(int id);

        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        Task<IEnumerable<Product>> SearchAsync(string keyword);

        Task<IEnumerable<Product>> FilterAsync(decimal? minPrice, decimal? maxPrice);
    }
}
