namespace backend.Configurations
{
    public interface IJwtProvider
    {
        /// <summary>
        /// Generates a JWT for a specified customer.
        /// </summary>
        /// <param name="Username">The unique identifier of the customer.</param>
        /// <param name="Email">The email of the customer.</param>
        /// <returns>A JWT string.</returns>
        /// <remarks>
        /// The token includes claims for the customer's ID and email. It is signed with a secret key and valid for a limited time.
        /// </remarks>
        public string Generate(string Username, string Email, List<string>Roles);
    }
}
