using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<Product?> GetProductWithDetailsAsync(int productId);
    }
}
