namespace backend.Services
{
    public interface IIdentityService
    {
        Task<string> LoginAsync(string username, string password);
        Task<bool> RegisterUserAsync(string username, string password, string email);
        Task<bool> AssignRoleToUserAsync(int userId, string roleName);
    }
}
