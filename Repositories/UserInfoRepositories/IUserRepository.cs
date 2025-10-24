using backend.Entities.UserInfo;

namespace backend.Repositories.UserInfoRepositories
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);

    }
}
