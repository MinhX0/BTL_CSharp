
namespace backend.Configurations
{
    public class JwtProvider : IJwtProvider
    {
        public JwtProvider()
        {

        }

        public string Generate(string Username, string Email, List<string> Roles)
        {
            // TODO: Implement JWT generation using configured signing key and claims
            throw new NotImplementedException();
        }
    }
}
