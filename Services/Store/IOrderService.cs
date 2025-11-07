using backend.Entities.Store;

namespace backend.Services.Store
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
        Task<Order> CreateAsync(Order order, IEnumerable<OrderDetail> orderDetails);
        Task<bool> UpdateStatusAsync(int orderId, string status);
    }
}
