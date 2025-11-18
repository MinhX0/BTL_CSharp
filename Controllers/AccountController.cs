using backend.Repositories.Store;
using backend.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICustomerRepository _customerRepository;

        public AccountController(IAdminRepository adminRepository, ICustomerRepository customerRepository)
        {
            _adminRepository = adminRepository;
            _customerRepository = customerRepository;
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

            return RedirectToAction("MyAccount");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccount(MyAccountViewModel model)
        {
            if (!Request.Cookies.TryGetValue("AuthCustomerId", out var userIdStr) || !int.TryParse(userIdStr, out var userId))
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
        public async Task<IActionResult> ChangePassword(int userId, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (!Request.Cookies.TryGetValue("AuthCustomerId", out var userIdStr) || !int.TryParse(userIdStr, out var cookieUserId) || cookieUserId != userId)
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
            // First check admin accounts (admin login uses username)
            var admin = await _adminRepository.GetByUsernameAsync(model.Username);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            if (admin is not null)
            {
                // For now passwords are compared as plaintext - replace with proper hashing
                if (admin.PasswordHash != model.Password)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }

                Response.Cookies.Append("AuthAdminId", admin.AdminId.ToString(), cookieOptions);
                // Redirect admins to the admin area/controller
                return RedirectToAction("Index", "Admin");
            }

            // Otherwise try customer login (customers log in with username)
            var customer = await _customerRepository.GetByUsernameAsync(model.Username);
            if (customer is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            if (customer.PasswordHash != model.Password)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            Response.Cookies.Append("AuthCustomerId", customer.CustomerId.ToString(), cookieOptions);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("MyAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("AuthCustomerId"))
            {
                Response.Cookies.Delete("AuthCustomerId");
            }

            if (Request.Cookies.ContainsKey("AuthAdminId"))
            {
                Response.Cookies.Delete("AuthAdminId");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            if (!Request.Cookies.TryGetValue("AuthCustomerId", out var userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login", new { returnUrl = Url.Action("MyAccount") });
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer is null)
            {
                // invalid cookie
                Response.Cookies.Delete("AuthCustomerId");
                return RedirectToAction("Login");
            }

            var vm = new MyAccountViewModel
            {
                UserId = customer.CustomerId,
                Username = customer.Username,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone ?? string.Empty,
                Address = customer.Address ?? string.Empty
            };

            return View(vm);
        }
    }
}
