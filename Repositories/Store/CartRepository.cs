using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByCustomerAsync(int customerId)
        {
            return await _dbSet
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Product)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(int customerId, int productId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CustomerId == customerId && c.ProductId == productId);
        }
    }
}
