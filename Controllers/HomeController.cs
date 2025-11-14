using backend.Entities.Store;
using backend.Services.Store;
using backend.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = (await _categoryService.GetAllAsync()).ToList();
            var products = (await _productService.GetAllAsync()).ToList();

            var productsByCategory = products
                .GroupBy(p => p.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            var categorySummaries = categories
                .Select(category => new CategorySummaryViewModel
                {
                    CategoryId = category.CategoryId,
                    Name = category.CategoryName,
                    ProductCount = productsByCategory.TryGetValue(category.CategoryId, out var count) ? count : 0,
                    ImageUrl = BuildCategoryImagePath(category)
                })
                .OrderByDescending(c => c.ProductCount)
                .ThenBy(c => c.Name)
                .Take(6)
                .ToList();

            var categoriesLookup = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

            var trendingProducts = products
                .Where(p => p.IsActive && p.DiscountPrice.HasValue)
                .OrderBy(p => p.DiscountPrice)
                .ThenByDescending(p => p.Price)
                .Select(product => ToProductCardViewModel(product, categoriesLookup))
                .Take(8)
                .ToList();

            var newArrivals = products
                .OrderByDescending(p => p.CreatedDate)
                .ThenByDescending(p => p.ProductId)
                .Select(product => ToProductCardViewModel(product, categoriesLookup))
                .Take(8)
                .ToList();

            var viewModel = new IndexViewModel
            {
                Categories = categorySummaries,
                TrendingProducts = trendingProducts,
                NewArrivals = newArrivals
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Shop(int? categoryId)
        {
            var categories = await _categoryService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            var categoriesLookup = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

            var filteredProducts = products.Where(p => p.IsActive);
            
            if (categoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == categoryId.Value);
            }

            var viewModel = new ShopViewModel
            {
                Products = filteredProducts
                    .Select(product => ToProductCardViewModel(product, categoriesLookup))
                    .ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductWithDetailsAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            var categoriesLookup = (await _categoryService.GetAllAsync())
                .ToDictionary(c => c.CategoryId, c => c.CategoryName);

            var relatedProducts = (await _productService.GetByCategoryAsync(product.CategoryId))
                .Where(p => p.ProductId != product.ProductId && p.IsActive)
                .OrderBy(p => p.DiscountPrice ?? long.MaxValue)
                .ThenByDescending(p => p.CreatedDate)
                .Select(p => ToProductCardViewModel(p, categoriesLookup))
                .Take(4)
                .ToList();

            var categoryName = categoriesLookup.TryGetValue(product.CategoryId, out var name)
                ? name
                : product.Category?.CategoryName;

            var viewModel = new ProductDetailViewModel
            {
                ProductId = product.ProductId,
                Name = product.ProductName,
                CategoryName = categoryName,
                Brand = product.Brand,
                Price = product.Price,
                OriginalPrice = product.DiscountPrice,
                Discount = CalculateDiscountPercentage(product.Price, product.DiscountPrice),
                ImageUrl = BuildProductImagePath(product),
                Description = string.IsNullOrWhiteSpace(product.Description)
                    ? "No description available for this watch yet."
                    : product.Description,
                Gender = product.Gender,
                Origin = product.Origin,
                MovementType = product.MovementType,
                Material = product.Material,
                StockQuantity = product.StockQuantity,
                Specifications = BuildSpecifications(product),
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        private static ProductCardViewModel ToProductCardViewModel(Product product, IReadOnlyDictionary<int, string> categoriesLookup)
        {
            return new ProductCardViewModel
            {
                ProductId = product.ProductId,
                Name = product.ProductName,
                CategoryName = categoriesLookup.TryGetValue(product.CategoryId, out var name) ? name : null,
                ImageUrl = BuildProductImagePath(product),
                Price = product.Price,
                OriginalPrice = product.DiscountPrice,
                Discount = CalculateDiscountPercentage(product.Price, product.DiscountPrice)
            };
        }

        private static string BuildProductImagePath(Product product)
        {
            var imagePath = product.Img?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return "~/template/img/product-1.jpg";
            }

            imagePath = imagePath.Replace("\\", "/");

            if (imagePath.StartsWith("~/", StringComparison.Ordinal))
            {
                return imagePath;
            }

            if (imagePath.StartsWith("/", StringComparison.Ordinal))
            {
                return "~" + imagePath;
            }

            // Map to external product images directory
            var fileName = Path.GetFileName(imagePath);
            return $"~/images/products/{fileName}";
        }

        private static string BuildCategoryImagePath(Category category)
        {
            var imagePath = category.Img?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return "~/template/img/cat-1.jpg";
            }

            imagePath = imagePath.Replace("\\", "/");

            if (imagePath.StartsWith("~/", StringComparison.Ordinal))
            {
                return imagePath;
            }

            if (imagePath.StartsWith("/", StringComparison.Ordinal))
            {
                return "~" + imagePath;
            }

            // Map to external category images directory
            var fileName = Path.GetFileName(imagePath);
            return $"~/images/categories/{fileName}";
        }

        private static double CalculateDiscountPercentage(long price, long? discountPrice)
        {
            if (!discountPrice.HasValue || discountPrice.Value >= price || discountPrice.Value <= 0)
            {
                return 0;
            }

            var discount = ((double)(price - discountPrice.Value) / price) * 100;
            return Math.Round(discount, 2);
        }

        private static IReadOnlyList<ProductSpecificationViewModel> BuildSpecifications(Product product)
        {
            var specifications = new List<ProductSpecificationViewModel>();

            void AddSpec(string label, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    specifications.Add(new ProductSpecificationViewModel
                    {
                        Label = label,
                        Value = value
                    });
                }
            }

            AddSpec("Brand", product.Brand);
            AddSpec("Collection", product.Category?.CategoryName);
            AddSpec("Gender", product.Gender);
            AddSpec("Origin", product.Origin);
            AddSpec("Movement", product.MovementType);
            AddSpec("Material", product.Material);

            specifications.Add(new ProductSpecificationViewModel
            {
                Label = "Stock",
                Value = product.StockQuantity.ToString()
            });

            specifications.Add(new ProductSpecificationViewModel
            {
                Label = "Added",
                Value = product.CreatedDate.ToString("MMMM dd, yyyy")
            });

            return specifications;
        }
    }
}
