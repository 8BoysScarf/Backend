using _8Boys.Models;

namespace _8Boys.Repositry
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetUserCartAsync(string userId);
    }
}
