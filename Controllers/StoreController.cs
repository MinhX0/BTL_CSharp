using backend.Entities.Store;
using backend.Services.Store;
using backend.ViewModels.Home;
using backend.ViewModels.Store;
using backend.Repositories.Store;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    public class StoreController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ICustomerRepository _customerRepository;

        public StoreController(
            IProductService productService,
            ICategoryService categoryService,
            ICartService cartService,
            IOrderService orderService,
            ICustomerRepository customerRepository)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
            _customerRepository = customerRepository;
        }

        public IActionResult Index()
        {
            // Redirect homepage to the Home controller's Index action — the Store controller now focuses on shop actions only.
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Shop(int? categoryId, string? search)
        {
            var categories = await _categoryService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            var categoriesLookup = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

            var filteredProducts = products.Where(p => p.IsActive);
            
            if (categoryId.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredProducts = filteredProducts.Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var viewModel = new ShopViewModel
            {
                Products = filteredProducts
                    .Select(product => ToProductCardViewModel(product, categoriesLookup))
                    .ToList(),
                SearchTerm = search,
                CategoryId = categoryId
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

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cart()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Cart") });
            }

            var cartItems = await _cartService.GetCustomerCartAsync(customerId.Value);
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var categoriesLookup = categories.ToDictionary(c => c.CategoryId, c => c.CategoryName);

            var vm = new CartViewModel
            {
                Items = cartItems.Select(ci =>
                {
                    var product = products.First(p => p.ProductId == ci.ProductId);
                    var unit = product.DiscountPrice.HasValue && product.DiscountPrice.Value > 0 ? product.DiscountPrice.Value : product.Price;
                    return new CartItemViewModel
                    {
                        ProductId = product.ProductId,
                        Name = product.ProductName,
                        ImageUrl = BuildProductImagePath(product),
                        Quantity = ci.Quantity,
                        UnitPrice = unit,
                        LineTotal = unit * ci.Quantity,
                        CategoryName = categoriesLookup.TryGetValue(product.CategoryId, out var cname) ? cname : null
                    };
                }).ToList()
            };

            return View(vm);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> Checkout()
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout") });
            }
            var customer = await _customerRepository.GetByIdAsync(customerId.Value);
            var cartItems = await _cartService.GetCustomerCartAsync(customerId.Value);
            if (!cartItems.Any())
            {
                TempData["Info"] = "Your cart is empty.";
                return RedirectToAction("Shop");
            }
            var products = await _productService.GetAllAsync();

            var checkoutVm = new CheckoutViewModel
            {
                Items = cartItems.Select(ci =>
                {
                    var product = products.First(p => p.ProductId == ci.ProductId);
                    var unit = product.DiscountPrice.HasValue && product.DiscountPrice.Value > 0 ? product.DiscountPrice.Value : product.Price;
                    return new CartItemViewModel
                    {
                        ProductId = product.ProductId,
                        Name = product.ProductName,
                        ImageUrl = BuildProductImagePath(product),
                        Quantity = ci.Quantity,
                        UnitPrice = unit,
                        LineTotal = unit * ci.Quantity
                    };
                }).ToList(),
                ShippingAddress = customer?.Address ?? string.Empty
            };
            return View(checkoutVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                if (isAjax)
                {
                    return Unauthorized();
                }
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Detail", new { id = productId }) });
            }

            var product = await _productService.GetProductWithDetailsAsync(productId);
            if (product == null || !product.IsActive)
            {
                if (isAjax)
                {
                    return BadRequest(new { message = "Sản phẩm không khả dụng." });
                }
                TempData["Error"] = "Product not available.";
                return RedirectToAction("Shop");
            }

            if (quantity <= 0)
            {
                quantity = 1;
            }
            if (quantity > product.StockQuantity)
            {
                quantity = product.StockQuantity;
            }

            await _cartService.AddOrUpdateItemAsync(customerId.Value, productId, quantity);
            
            if (isAjax)
            {
                return Ok(new { message = "Đã thêm vào giỏ hàng!" });
            }
            
            TempData["Success"] = "Added to cart.";
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCart(int productId, int quantity)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Cart") });
            }
            if (quantity <= 0)
            {
                await _cartService.RemoveItemAsync(customerId.Value, productId);
                TempData["Info"] = "Item removed.";
                return RedirectToAction("Cart");
            }
            await _cartService.AddOrUpdateItemAsync(customerId.Value, productId, quantity);
            TempData["Success"] = "Cart updated.";
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Cart") });
            }
            await _cartService.RemoveItemAsync(customerId.Value, productId);
            TempData["Info"] = "Item removed.";
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Checkout") });
            }
            var cartItems = await _cartService.GetCustomerCartAsync(customerId.Value);
            if (!cartItems.Any())
            {
                TempData["Info"] = "Your cart is empty.";
                return RedirectToAction("Shop");
            }
            if (string.IsNullOrWhiteSpace(model.ShippingAddress))
            {
                TempData["Error"] = "Shipping address required.";
                return RedirectToAction("Checkout");
            }
            var products = await _productService.GetAllAsync();
            var orderDetails = new List<OrderDetail>();
            long total = 0;
            foreach (var item in cartItems)
            {
                var p = products.FirstOrDefault(pr => pr.ProductId == item.ProductId);
                if (p == null) continue;
                var unit = p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 ? p.DiscountPrice.Value : p.Price;
                total += unit * item.Quantity;
                orderDetails.Add(new OrderDetail
                {
                    ProductId = p.ProductId,
                    Quantity = item.Quantity,
                    Price = unit
                });
            }
            var order = new Order
            {
                CustomerId = customerId.Value,
                ShippingAddress = model.ShippingAddress,
                TotalAmount = total,
                PaymentMethod = model.PaymentMethod ?? "CashOnDelivery",
                Status = "Pending",
                OrderDate = DateTime.UtcNow
            };
            await _orderService.CreateAsync(order, orderDetails);
            await _cartService.ClearCartAsync(customerId.Value);
            TempData["Success"] = $"Order #{order.OrderId} placed successfully.";
            return RedirectToAction("Detail", "Store", new { id = orderDetails.First().ProductId }); // simple redirect; could go to order confirmation page
        }

        private int? GetCurrentCustomerId()
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(role) || !role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            var idStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idStr, out var id)) return null;
            return id;
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

            AddSpec("Nhãn hiệu", product.Brand);
            AddSpec("Loại", product.Category?.CategoryName);
            AddSpec("Giới tính", product.Gender);
            AddSpec("Nguồn gốc", product.Origin);
            AddSpec("Vòng xoay", product.MovementType);
            AddSpec("Chất liệu", product.Material);

            specifications.Add(new ProductSpecificationViewModel
            {
                Label = "Số lượng",
                Value = product.StockQuantity.ToString()
            });

            specifications.Add(new ProductSpecificationViewModel
            {
                Label = "Đã thêm ngày",
                Value = product.CreatedDate.ToString("MMMM dd, yyyy")
            });

            return specifications;
        }
    }
}
