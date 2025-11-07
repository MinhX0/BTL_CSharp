using backend.Entities.Store;

namespace backend.Repositories.Store
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetByUsernameAsync(string username);
    }
}
