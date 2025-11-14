using backend.Services.Store;
using Microsoft.AspNetCore.Mvc;

namespace backend.ViewComponents
{
    public class CategoryNavigationViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryNavigationViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }
    }
}
