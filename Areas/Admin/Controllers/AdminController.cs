using Microsoft.AspNetCore.Mvc;
using Veritabanı_proje.Data;
using Veritabanı_proje.Models;
using Veritabanı_proje.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Veritabanı_proje.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin paneli ana sayfası
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalTinyHouses = await _context.TinyHouses.CountAsync(),
                TotalReservations = await _context.Reservations.CountAsync()
            };
            return View(model);
        }

        // Kullanıcı yönetimi
        public async Task<IActionResult> Users(string searchString)
        {
            var users = from u in _context.Users select u;
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString));
            }
            return View(await users.ToListAsync());
        }

        // İlan yönetimi
        public async Task<IActionResult> TinyHouses()
        {
            var houses = await _context.TinyHouses.ToListAsync();
            return View(houses);
        }

        // Mali raporlar
        public IActionResult Reports()
        {
            return View();
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Users));
            }
            return View(user);
        }

        // GET: Admin/DeleteUser/5
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }
    }
}
