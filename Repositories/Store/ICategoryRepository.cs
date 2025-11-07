using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetCategoryWithProductsAsync(int categoryId);
    }
}
