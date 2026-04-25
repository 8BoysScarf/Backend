using _8Boys.Models;

namespace _8Boys.Repositry
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(int id);

        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    }
}
