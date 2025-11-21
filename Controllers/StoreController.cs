using backend.Entities.Store;
using backend.Services.Store;
using backend.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public StoreController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            // Redirect homepage to the Home controller's Index action — the Store controller now focuses on shop actions only.
            return RedirectToAction("Index", "Home");
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
            // Return a clean placeholder view while the original Checkout.cshtml is being repaired.
            return View("CheckoutClean");
        }

        public IActionResult Contact()
        {
            // Contact page belongs to Home controller; redirect to central location
            return RedirectToAction("Contact", "Home");
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

            if (imagePath.StartsWith("~/", System.StringComparison.Ordinal))
            {
                return imagePath;
            }

            if (imagePath.StartsWith("/", System.StringComparison.Ordinal))
            {
                return "~" + imagePath;
            }

            // Map to external product images directory
            var fileName = System.IO.Path.GetFileName(imagePath);
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

            if (imagePath.StartsWith("~/", System.StringComparison.Ordinal))
            {
                return imagePath;
            }

            if (imagePath.StartsWith("/", System.StringComparison.Ordinal))
            {
                return "~" + imagePath;
            }

            // Map to external category images directory
            var fileName = System.IO.Path.GetFileName(imagePath);
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
