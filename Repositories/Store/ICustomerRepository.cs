using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> GetByUsernameAsync(string username);
        Task<Customer?> GetCustomerWithOrdersAsync(int customerId);
    }
}
