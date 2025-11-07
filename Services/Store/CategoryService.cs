using backend.Entities.Store;
using backend.Repositories.Store;

namespace backend.Services.Store
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _categoryRepository.AddAsync(category);
            return category;
        }

        public async Task<bool> UpdateAsync(Category category)
        {
            var existing = await _categoryRepository.GetByIdAsync(category.CategoryId);
            if (existing is null)
            {
                return false;
            }

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.Img = category.Img;

            _categoryRepository.Update(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing is null)
            {
                return false;
            }

            _categoryRepository.Delete(existing);
            return true;
        }
    }
}
