using Microsoft.AspNetCore.Mvc;
using Veritabanı_proje.Models;
using Veritabanı_proje.Data;
using Microsoft.EntityFrameworkCore;
using Veritabanı_proje.Models.ViewModels;
using System.Threading.Tasks;

namespace Veritabanı_proje.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Giriş sayfası
        public IActionResult Login()
        {
            return View();
        }

        // Kayıt sayfası
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    Phone = model.Phone,
                    Role = model.Role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserRole", ((int)user.Role).ToString());
                    HttpContext.Session.SetString("UserName", user.FirstName);

                    switch (user.Role)
                    {
                        case Role.Admin:
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        case Role.EvSahibi:
                            return RedirectToAction("Index", "EvSahibi");
                        case Role.Kiraci:
                            return RedirectToAction("Index", "Kiraci");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Geçersiz email veya şifre");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ListUsers (Sadece test için)
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Role,
                        u.CreatedAt
                    })
                    .ToListAsync();

                Console.WriteLine("\nKayıtlı Kullanıcılar:");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Ad: {user.FirstName} {user.LastName}, Email: {user.Email}, Rol: {user.Role}, Kayıt Tarihi: {user.CreatedAt}");
                }

                return Json(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }
    }
} 