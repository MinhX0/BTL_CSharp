using backend.Entities.Store;

namespace backend.Services.Store
{
    public interface IAdminService
    {
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(int id);
        Task<Admin?> GetByUsernameAsync(string username);
        Task<Admin> CreateAsync(Admin admin);
        Task<bool> UpdateAsync(Admin admin);
        Task<bool> DeleteAsync(int id);
    }
}
