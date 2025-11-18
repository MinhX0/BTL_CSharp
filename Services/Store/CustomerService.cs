using backend.Entities.Store;
using backend.Repositories.Store;

namespace backend.Services.Store
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _customerRepository.GetByEmailAsync(email);
        }

        public async Task<Customer?> GetByUsernameAsync(string username)
        {
            return await _customerRepository.GetByUsernameAsync(username);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            await _customerRepository.AddAsync(customer);
            return customer;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            var existing = await _customerRepository.GetByIdAsync(customer.CustomerId);
            if (existing is null)
            {
                return false;
            }

            existing.FullName = customer.FullName;
            existing.Email = customer.Email;
            existing.PasswordHash = customer.PasswordHash;
            existing.Phone = customer.Phone;
            existing.Address = customer.Address;

            _customerRepository.Update(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing is null)
            {
                return false;
            }

            _customerRepository.Delete(existing);
            return true;
        }
    }
}
