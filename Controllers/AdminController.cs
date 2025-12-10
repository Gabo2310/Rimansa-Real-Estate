using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RimansaRealEstate.Data;
using RimansaRealEstate.Models;
using System.Security.Claims;

namespace RimansaRealEstate.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == model.Username && a.IsActive);

                if (admin != null && BCrypt.Net.BCrypt.Verify(model.Password, admin.PasswordHash))
                {
                    admin.LastLogin = DateTime.Now;
                    await _context.SaveChangesAsync();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, admin.Username),
                        new Claim(ClaimTypes.Email, admin.Email),
                        new Claim("FullName", admin.FullName),
                        new Claim(ClaimTypes.Role, "Admin")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, 
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) 
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Dashboard");
                }

                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var totalProperties = await _context.Properties.CountAsync();
            var activeProperties = await _context.Properties.CountAsync(p => p.IsActive);
            var forSale = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Venta);
            var forRent = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Alquiler);

            ViewBag.TotalProperties = totalProperties;
            ViewBag.ActiveProperties = activeProperties;
            ViewBag.ForSale = forSale;
            ViewBag.ForRent = forRent;

            var recentProperties = await _context.Properties
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(recentProperties);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}