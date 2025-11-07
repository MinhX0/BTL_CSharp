using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderDetail>> GetDetailsByOrderAsync(int orderId)
        {
            return await _dbSet
                .Where(od => od.OrderId == orderId)
                .Include(od => od.Product)
                .ToListAsync();
        }
    }
}
