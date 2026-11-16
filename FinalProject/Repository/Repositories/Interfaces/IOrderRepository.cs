using Domain.Entities;

namespace Repository.Repositories.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<Order>> GetAllWithItemsAsync();
        Task<List<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetWithItemsAsync(int id);
        Task<Order?> GetBySessionIdAsync(string sessionId);
        Task AddAsync(Order order);
        Task SaveChangesAsync();
    }
}
