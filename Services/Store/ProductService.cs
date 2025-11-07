using backend.Entities.Store;
using backend.Repositories.Store;

namespace backend.Services.Store
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _productRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existing = await _productRepository.GetByIdAsync(product.ProductId);
            if (existing is null)
            {
                return false;
            }

            existing.ProductName = product.ProductName;
            existing.CategoryId = product.CategoryId;
            existing.Brand = product.Brand;
            existing.Price = product.Price;
            existing.Discount = product.Discount;
            existing.Description = product.Description;
            existing.Gender = product.Gender;
            existing.Origin = product.Origin;
            existing.MovementType = product.MovementType;
            existing.Material = product.Material;
            existing.Img = product.Img;
            existing.StockQuantity = product.StockQuantity;

            _productRepository.Update(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing is null)
            {
                return false;
            }

            _productRepository.Delete(existing);
            return true;
        }
    }
}
