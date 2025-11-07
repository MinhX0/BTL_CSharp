using backend.Entities.Store;
using backend.Repositories.Store;

namespace backend.Services.Store
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<Cart>> GetCustomerCartAsync(int customerId)
        {
            return await _cartRepository.GetCartItemsByCustomerAsync(customerId);
        }

        public async Task<Cart> AddOrUpdateItemAsync(int customerId, int productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            }

            var cartItem = await _cartRepository.GetCartItemAsync(customerId, productId);
            if (cartItem is null)
            {
                cartItem = new Cart
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.UtcNow
                };

                await _cartRepository.AddAsync(cartItem);
                return cartItem;
            }

            cartItem.Quantity = quantity;
            cartItem.AddedDate = DateTime.UtcNow;
            _cartRepository.Update(cartItem);
            return cartItem;
        }

        public async Task<bool> RemoveItemAsync(int customerId, int productId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(customerId, productId);
            if (cartItem is null)
            {
                return false;
            }

            _cartRepository.Delete(cartItem);
            return true;
        }

        public async Task<bool> ClearCartAsync(int customerId)
        {
            var items = await _cartRepository.GetCartItemsByCustomerAsync(customerId);
            var removed = false;
            foreach (var item in items)
            {
                _cartRepository.Delete(item);
                removed = true;
            }

            return removed;
        }
    }
}
