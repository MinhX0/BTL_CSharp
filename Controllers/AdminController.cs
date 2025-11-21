using backend.Repositories.Store;
using backend.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Authorize(Roles = "Owner,Manager,Staff")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public AdminController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ICustomerRepository customerRepository,
            IOrderRepository orderRepository,
            IOrderDetailRepository orderDetailRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        // Dashboard
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            var customers = await _customerRepository.GetAllAsync();
            var orders = await _orderRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var pendingOrders = orders.Count(o => o.Status?.ToLower() == "pending");

            var recentOrders = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o =>
                {
                    var customer = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId);
                    return new RecentOrderViewModel
                    {
                        OrderId = o.OrderId,
                        CustomerName = customer?.FullName ?? "Unknown",
                        TotalAmount = o.TotalAmount,
                        Status = o.Status ?? "Pending",
                        OrderDate = o.OrderDate
                    };
                })
                .ToList();

            var vm = new DashboardViewModel
            {
                TotalProducts = products.Count(),
                TotalCustomers = customers.Count(),
                TotalOrders = orders.Count(),
                TotalRevenue = totalRevenue,
                PendingOrders = pendingOrders,
                TotalCategories = categories.Count(),
                RecentOrders = recentOrders
            };

            return View(vm);
        }

        // Products Management
        [HttpGet]
        public async Task<IActionResult> Products(string? search, int page = 1, int pageSize = 20)
        {
            var allProducts = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                allProducts = allProducts.Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = allProducts.Count();
            var products = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductItemViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.ProductName,
                    CategoryName = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId)?.CategoryName,
                    Price = p.Price,
                    Stock = p.StockQuantity,
                    ImageUrl = p.Img,
                    IsActive = true
                })
                .ToList();

            var vm = new ProductListViewModel
            {
                Products = products,
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = pageSize,
                SearchTerm = search
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var vm = new ProductFormViewModel
            {
                Categories = categories.Select(c => new CategoryOptionViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.CategoryName
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                model.Categories = categories.Select(c => new CategoryOptionViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.CategoryName
                }).ToList();
                return View(model);
            }

            var product = new backend.Entities.Store.Product
            {
                ProductName = model.Name,
                Description = model.Description,
                Price = model.Price,
                StockQuantity = model.Stock,
                CategoryId = model.CategoryId,
                Img = model.ImageUrl
            };

            await _productRepository.AddAsync(product);

            TempData["Success"] = "Product created successfully.";
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            var vm = new ProductFormViewModel
            {
                ProductId = product.ProductId,
                Name = product.ProductName,
                Description = product.Description ?? string.Empty,
                Price = product.Price,
                Stock = product.StockQuantity,
                CategoryId = product.CategoryId,
                ImageUrl = product.Img,
                Categories = categories.Select(c => new CategoryOptionViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.CategoryName
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                model.Categories = categories.Select(c => new CategoryOptionViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.CategoryName
                }).ToList();
                return View(model);
            }

            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            product.ProductName = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.StockQuantity = model.Stock;
            product.CategoryId = model.CategoryId;
            product.Img = model.ImageUrl;

            _productRepository.Update(product);

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _productRepository.Delete(product);

            TempData["Success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Products));
        }

        // Customers Management
        [HttpGet]
        public async Task<IActionResult> Customers(string? search, int page = 1, int pageSize = 20)
        {
            var allCustomers = await _customerRepository.GetAllAsync();
            var orders = await _orderRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                allCustomers = allCustomers.Where(c =>
                    c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null && c.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Username != null && c.Username.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var totalCount = allCustomers.Count();
            var customers = allCustomers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerItemViewModel
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                    Email = c.Email ?? string.Empty,
                    Username = c.Username,
                    Phone = c.Phone,
                    TotalOrders = orders.Count(o => o.CustomerId == c.CustomerId),
                    TotalSpent = orders.Where(o => o.CustomerId == c.CustomerId).Sum(o => o.TotalAmount)
                })
                .ToList();

            var vm = new CustomerListViewModel
            {
                Customers = customers,
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = pageSize,
                SearchTerm = search
            };

            return View(vm);
        }

        // Categories Management
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            var vm = new CategoryListViewModel
            {
                Categories = categories.Select(c => new CategoryItemViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.CategoryName,
                    ImageUrl = c.Img,
                    ProductCount = products.Count(p => p.CategoryId == c.CategoryId)
                }).ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = new backend.Entities.Store.Category
            {
                CategoryName = model.Name,
                Img = model.ImageUrl
            };

            await _categoryRepository.AddAsync(category);

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var vm = new CategoryFormViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.CategoryName,
                ImageUrl = category.Img
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
            if (category == null)
            {
                return NotFound();
            }

            category.CategoryName = model.Name;
            category.Img = model.ImageUrl;

            _categoryRepository.Update(category);

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category has products
            var products = await _productRepository.GetAllAsync();
            if (products.Any(p => p.CategoryId == id))
            {
                TempData["Error"] = "Cannot delete category with existing products.";
                return RedirectToAction(nameof(Categories));
            }

            _categoryRepository.Delete(category);

            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Categories));
        }

        // Orders Management
        [HttpGet]
        public async Task<IActionResult> Orders(string? status, int page = 1, int pageSize = 20)
        {
            var allOrders = await _orderRepository.GetAllAsync();
            var customers = await _customerRepository.GetAllAsync();
            var orderDetails = await _orderDetailRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(status))
            {
                allOrders = allOrders.Where(o => o.Status != null && o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = allOrders.Count();
            var orders = allOrders
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderItemViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = customers.FirstOrDefault(c => c.CustomerId == o.CustomerId)?.FullName ?? "Unknown",
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status ?? "Pending",
                    ItemCount = orderDetails.Count(od => od.OrderId == o.OrderId)
                })
                .ToList();

            var vm = new OrderListViewModel
            {
                Orders = orders,
                TotalCount = totalCount,
                PageIndex = page,
                PageSize = pageSize,
                StatusFilter = status
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
            var orderDetails = (await _orderDetailRepository.GetAllAsync())
                .Where(od => od.OrderId == id)
                .ToList();

            var products = await _productRepository.GetAllAsync();

            var vm = new OrderDetailViewModel
            {
                OrderId = order.OrderId,
                CustomerName = customer?.FullName ?? "Unknown",
                CustomerEmail = customer?.Email ?? string.Empty,
                CustomerPhone = customer?.Phone,
                ShippingAddress = order.ShippingAddress,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status ?? "Pending",
                Items = orderDetails.Select(od => new OrderDetailItemViewModel
                {
                    ProductName = products.FirstOrDefault(p => p.ProductId == od.ProductId)?.ProductName ?? "Unknown Product",
                    Quantity = od.Quantity,
                    UnitPrice = od.Price,
                    Subtotal = od.Quantity * od.Price
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            _orderRepository.Update(order);

            TempData["Success"] = "Order status updated successfully.";
            return RedirectToAction(nameof(OrderDetail), new { id = orderId });
        }
    }
}
