using backend.Repositories.Store;
using backend.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace backend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public AccountController(IAdminRepository adminRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _adminRepository = adminRepository;
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var vm = new LoginViewModel { ReturnUrl = returnUrl };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new backend.ViewModels.Account.RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(backend.ViewModels.Account.RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingByEmail = await _customerRepository.GetByEmailAsync(model.Email);
            var existingByUsername = await _customerRepository.GetByUsernameAsync(model.Username);
            if (existingByEmail is not null || existingByUsername is not null)
            {
                ModelState.AddModelError(string.Empty, "This email or username is already registered. Try logging in.");
                return View(model);
            }

            var customer = new backend.Entities.Store.Customer
            {
                FullName = model.FullName,
                Email = model.Email,
                Username = model.Username,
                PasswordHash = model.Password, // NOTE: use hashing in production
                Phone = model.Phone,
                Address = model.Address
            };

            await _customerRepository.AddAsync(customer);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("AuthCustomerId", customer.CustomerId.ToString(), cookieOptions);

            // Sign in newly registered customer
            var customerClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                new Claim(ClaimTypes.Name, customer.Username ?? customer.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var customerIdentity = new ClaimsIdentity(customerClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(customerIdentity), new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

            return RedirectToAction("MyAccount");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateAccount(MyAccountViewModel model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("MyAccount") });
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer is null)
            {
                Response.Cookies.Delete("AuthCustomerId");
                return RedirectToAction("Login");
            }

            customer.FullName = model.FullName ?? customer.FullName;
            customer.Email = model.Email ?? customer.Email;
            customer.Phone = model.Phone ?? customer.Phone;
            customer.Address = model.Address ?? customer.Address;

            _customerRepository.Update(customer);

            return RedirectToAction("MyAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword(int userId, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var cookieUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(cookieUserIdStr) || !int.TryParse(cookieUserIdStr, out var cookieUserId) || cookieUserId != userId)
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("MyAccount") });
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer is null)
            {
                Response.Cookies.Delete("AuthCustomerId");
                return RedirectToAction("Login");
            }

            // Simple password check (plaintext against PasswordHash field). Replace with hashing in production.
            if (customer.PasswordHash != CurrentPassword)
            {
                TempData["ChangePasswordError"] = "Current password is incorrect.";
                return RedirectToAction("MyAccount");
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmPassword)
            {
                TempData["ChangePasswordError"] = "New password and confirmation do not match.";
                return RedirectToAction("MyAccount");
            }

            customer.PasswordHash = NewPassword;
            _customerRepository.Update(customer);

            TempData["ChangePasswordSuccess"] = "Password updated.";
            return RedirectToAction("MyAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            // Customer-only login; admin users must use /Admin/Login
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            // Customers log in with username
            var customer = await _customerRepository.GetByUsernameAsync(model.Username);
            if (customer is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password (customer accounts only). For admin use /Admin/Login.");
                return View(model);
            }

            if (customer.PasswordHash != model.Password)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            Response.Cookies.Append("AuthCustomerId", customer.CustomerId.ToString(), cookieOptions);

            // Sign in customer principal
            var customerClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                new Claim(ClaimTypes.Name, customer.Username ?? customer.Email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var customerIdentity = new ClaimsIdentity(customerClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(customerIdentity), new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

            // Persist session server-side so UI can read logged-in state even when cookie auth hasn't yet been established
            HttpContext.Session.SetString("AuthCustomerId", customer.CustomerId.ToString());
            HttpContext.Session.SetString("Username", customer.Username ?? customer.Email);

            // Carve out a server-side flag to indicate sign-in has completed
            HttpContext.Session.SetString("IsSignedIn", "1");

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.ContainsKey("AuthCustomerId"))
            {
                Response.Cookies.Delete("AuthCustomerId");
            }

            if (Request.Cookies.ContainsKey("AuthAdminId"))
            {
                Response.Cookies.Delete("AuthAdminId");
            }

            // Clear server-side session state
            HttpContext.Session.Remove("AuthCustomerId");
            HttpContext.Session.Remove("AuthAdminId");
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("IsSignedIn");

            // Sign out cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Block admin roles from MyAccount (they should use admin area explicitly)
            if (!string.IsNullOrWhiteSpace(role) && (role.Equals("Owner", StringComparison.OrdinalIgnoreCase) || role.Equals("Manager", StringComparison.OrdinalIgnoreCase) || role.Equals("Staff", StringComparison.OrdinalIgnoreCase)))
            {
                return Forbid();
            }
            if (string.IsNullOrWhiteSpace(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("MyAccount") });
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer is null)
            {
                // If we could not find a customer with the authenticated id, sign them out as an emergency measure.
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Response.Cookies.Delete("AuthCustomerId");
                HttpContext.Session.Remove("AuthCustomerId");
                HttpContext.Session.Remove("IsSignedIn");
                return RedirectToAction("Login", new { returnUrl = Url.Action("MyAccount") });
            }

            var orders = await _orderRepository.GetOrdersByCustomerAsync(customer.CustomerId);

            var vm = new MyAccountViewModel
            {
                UserId = customer.CustomerId,
                Username = customer.Username,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone ?? string.Empty,
                Address = customer.Address ?? string.Empty,
                Orders = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ItemCount = o.OrderDetails?.Count ?? 0,
                    PaymentMethod = o.PaymentMethod,
                    ShippingAddress = o.ShippingAddress
                }).OrderByDescending(o => o.OrderDate).ToList()
            };

            return View(vm);
        }
    }
}
