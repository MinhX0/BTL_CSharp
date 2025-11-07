using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
    }
}
