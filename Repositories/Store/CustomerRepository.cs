using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }
    }
}
