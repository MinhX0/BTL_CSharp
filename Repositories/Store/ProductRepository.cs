using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithDetailsAsync(int productId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }
    }
}
