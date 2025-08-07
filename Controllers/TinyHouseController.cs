using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using Veritabanı_proje.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Veritabanı_proje.Data;
using Microsoft.AspNetCore.Authorization;

namespace Veritabanı_proje.Controllers
{
    public class TinyHouseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TinyHouseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var houses = await _context.TinyHouses.ToListAsync();
            return View(houses);
        }

        // GET: TinyHouse/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TinyHouse/Create
        [HttpPost]
        public async Task<IActionResult> Create(TinyHouse tinyHouse)
        {
            if (ModelState.IsValid)
            {
                // User ID'yi int olarak al
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    tinyHouse.OwnerId = int.Parse(userIdClaim);
                }

                // Kullanıcının varlığını kontrol et
                var user = await _context.Users.FindAsync(tinyHouse.OwnerId);
                if (user == null)
                {
                    Console.WriteLine($"User with ID {tinyHouse.OwnerId} not found in database");
                    ModelState.AddModelError("", "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.");
                    return View(tinyHouse);
                }

                Console.WriteLine($"Found user: {user.FirstName} {user.LastName} (ID: {user.Id})");

                // Yeni TinyHouse nesnesini oluştur
                var newHouse = new TinyHouse
                {
                    Name = tinyHouse.Name,
                    Description = tinyHouse.Description,
                    Price = tinyHouse.Price,
                    Location = tinyHouse.Location,
                    SquareMeters = tinyHouse.SquareMeters,
                    OwnerId = tinyHouse.OwnerId,  // String olarak bırakıyoruz
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                Console.WriteLine($"Creating TinyHouse with properties:");
                Console.WriteLine($"Name: {newHouse.Name}");
                Console.WriteLine($"OwnerId: {newHouse.OwnerId}");
                Console.WriteLine($"Price: {newHouse.Price}");

                _context.TinyHouses.Add(newHouse);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(tinyHouse);
        }

        // Detay Görüntüleme
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var house = await _context.TinyHouses
                .Include(h => h.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
                return NotFound();
            
            return View(house);
        }

        // Düzenleme
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var house = await _context.TinyHouses.FindAsync(id);
            if (house == null)
                return NotFound();

            return View(house);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TinyHouse house)
        {
            if (id != house.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(house);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TinyHouseExists(house.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(house);
        }

        // Silme
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var house = await _context.TinyHouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
                return NotFound();

            return View(house);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _context.TinyHouses.FindAsync(id);
            if (house != null)
            {
                _context.TinyHouses.Remove(house);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TinyHouseExists(int id)
        {
            return _context.TinyHouses.Any(e => e.Id == id);
        }
    }
} 