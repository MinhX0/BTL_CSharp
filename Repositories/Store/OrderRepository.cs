using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }
    }
}
