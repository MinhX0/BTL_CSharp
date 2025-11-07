using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }
    }
}
