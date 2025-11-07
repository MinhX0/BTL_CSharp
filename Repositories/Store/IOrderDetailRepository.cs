using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IEnumerable<OrderDetail>> GetDetailsByOrderAsync(int orderId);
    }
}
