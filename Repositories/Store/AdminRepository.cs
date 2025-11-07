using backend.Entities.Store;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.Store
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(IdentityDbContext context) : base(context)
        {
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Username == username);
        }
    }
}
