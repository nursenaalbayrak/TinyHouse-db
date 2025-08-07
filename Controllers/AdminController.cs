using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veritabanı_proje.Data;
using Veritabanı_proje.Models;
using Veritabanı_proje.Models.ViewModels;

namespace Veritabanı_proje.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                ActiveUsers = await _context.Users.Where(u => u.IsActive).CountAsync(),
                TotalTinyHouses = await _context.TinyHouses.CountAsync(),
                // Rezervasyon ve ödeme sayıları eklenecek
            };

            return View(viewModel);
        }

        // Kullanıcı Yönetimi
        public async Task<IActionResult> Users(string searchString)
        {
            var users = from u in _context.Users
                       select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Email.Contains(searchString) 
                    || u.FirstName.Contains(searchString) 
                    || u.LastName.Contains(searchString));
            }

            return View(await users.ToListAsync());
        }

        // Kullanıcı Düzenleme
        public async Task<IActionResult> EditUser(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Users));
            }
            return View(user);
        }

        // Kullanıcı Silme
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // İlan Yönetimi
        public async Task<IActionResult> TinyHouses()
        {
            var houses = await _context.TinyHouses.ToListAsync();
            return View(houses);
        }

        // Mali Raporlar ve İstatistikler sayfası
        public IActionResult Reports()
        {
            return View();
        }
    }
} 