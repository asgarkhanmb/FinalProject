using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Data;
using Repository.Repositories.Interfaces;

namespace Repository.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        private IQueryable<Order> WithDetails() =>
            _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.AppUser);

        public async Task<List<Order>> GetAllWithItemsAsync()
        {
            return await WithDetails()
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserIdAsync(string userId)
        {
            return await WithDetails()
                .Where(o => o.AppUserId == userId)
                .OrderByDescending(o => o.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Order?> GetWithItemsAsync(int id)
        {
            return await WithDetails().FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetBySessionIdAsync(string sessionId)
        {
            return await WithDetails().FirstOrDefaultAsync(o => o.StripeSessionId == sessionId);
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
