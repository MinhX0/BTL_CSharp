using backend.Entities.Store;
using backend.Repositories.Store;

namespace backend.Services.Store
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _adminRepository.GetAllAsync();
        }

        public async Task<Admin?> GetByIdAsync(int id)
        {
            return await _adminRepository.GetByIdAsync(id);
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _adminRepository.GetByUsernameAsync(username);
        }

        public async Task<Admin> CreateAsync(Admin admin)
        {
            await _adminRepository.AddAsync(admin);
            return admin;
        }

        public async Task<bool> UpdateAsync(Admin admin)
        {
            var existing = await _adminRepository.GetByIdAsync(admin.AdminId);
            if (existing is null)
            {
                return false;
            }

            existing.Username = admin.Username;
            existing.PasswordHash = admin.PasswordHash;
            existing.Role = admin.Role;

            _adminRepository.Update(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _adminRepository.GetByIdAsync(id);
            if (existing is null)
            {
                return false;
            }

            _adminRepository.Delete(existing);
            return true;
        }
    }
}
