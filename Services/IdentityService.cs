using backend.Repositories.UserInfoRepositories;

namespace backend.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        public IdentityService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public Task<bool> AssignRoleToUserAsync(int userId, string roleName)
        {
            _userRepository.GetUserByUsernameAsync
        }

        public Task<string> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterUserAsync(string username, string password, string email)
        {
            throw new NotImplementedException();
        }
    }
}
