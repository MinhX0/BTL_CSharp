using System.ComponentModel.DataAnnotations;

namespace backend.ViewModels.Admin
{
    public class ProductListViewModel
    {
        public List<ProductItemViewModel> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class ProductItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public long Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductFormViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, long.MaxValue, ErrorMessage = "Price must be positive")]
        public long Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be positive")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public string? ImageUrl { get; set; }
        
        public List<CategoryOptionViewModel> Categories { get; set; } = new();
    }

    public class CategoryOptionViewModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
