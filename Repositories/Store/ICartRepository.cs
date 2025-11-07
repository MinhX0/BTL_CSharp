using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<IEnumerable<Cart>> GetCartItemsByCustomerAsync(int customerId);
        Task<Cart?> GetCartItemAsync(int customerId, int productId);
    }
}
