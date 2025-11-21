using backend.Repositories.Store;
using backend.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("Admin")]
    public class AdminAuthController : Controller
    {
        private readonly IAdminRepository _adminRepository;

        public AdminAuthController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View("Login", new AdminLoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var admin = await _adminRepository.GetByUsernameAsync(model.Username);
            if (admin == null || admin.PasswordHash != model.Password) // TODO: replace with hashing
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View("Login", model);
            }

            // Use the stored role value (Staff/Manager/Owner) so it matches the Authorize attribute
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, admin.AdminId.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, admin.Role ?? "Staff")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) }
            );

            HttpContext.Session.SetString("AuthAdminId", admin.AdminId.ToString());
            HttpContext.Session.SetString("Username", admin.Username);
            HttpContext.Session.SetString("IsSignedIn", "1");

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Admin");
        }

        // Secret minimal registration endpoint for quick testing
        [HttpGet("__register-secret")]
        [AllowAnonymous]
        public IActionResult RegisterSecret()
        {
            return View("RegisterSecret", new AdminRegisterViewModel());
        }

        [HttpPost("__register-secret")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterSecret(AdminRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("RegisterSecret", model);

            var existing = await _adminRepository.GetByUsernameAsync(model.Username);
            if (existing != null)
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return View("RegisterSecret", model);
            }

            var admin = new backend.Entities.Store.Admin
            {
                Username = model.Username,
                PasswordHash = model.Password, // TODO: hash
                Role = "Staff" // use 'Staff' to satisfy existing DB check constraint if case-sensitive
            };

            try
            {
                await _adminRepository.AddAsync(admin);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Surface a friendly error hinting at allowed role values
                ModelState.AddModelError(string.Empty, "Failed to create admin. Role constraint violated. Try using an existing role value (Staff/Manager/Owner). " + ex.InnerException?.Message);
                return View("RegisterSecret", model);
            }
            TempData["Success"] = "Admin registered with role 'Staff'. You can now login.";
            return RedirectToAction("Login");
        }
    }
}
