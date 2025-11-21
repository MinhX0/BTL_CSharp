using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels.Admin
{
    public class CategoryListViewModel
    {
        public List<CategoryItemViewModel> Categories { get; set; } = new();
    }

    public class CategoryItemViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int ProductCount { get; set; }
    }

    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
    }
}
