using backend.Entities.Store;

namespace backend.Services.Store
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetCustomerCartAsync(int customerId);
        Task<Cart> AddOrUpdateItemAsync(int customerId, int productId, int quantity);
        Task<bool> RemoveItemAsync(int customerId, int productId);
        Task<bool> ClearCartAsync(int customerId);
    }
}
