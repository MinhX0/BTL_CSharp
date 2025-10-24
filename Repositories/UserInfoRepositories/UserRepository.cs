using backend.Entities.UserInfo;
using backend.Persistance;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories.UserInfoRepositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IdentityDbContext context) : base(context) { }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbSet
             .Include(u => u.UserRoles)
             .ThenInclude(ur => ur.Role)
             .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _dbSet
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
            .ToListAsync();
        }
    }
}
