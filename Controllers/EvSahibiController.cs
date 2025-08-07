using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veritabanı_proje.Models;
using Veritabanı_proje.Data;
using Veritabanı_proje.Attributes;
using Veritabanı_proje.Models.ViewModels;

namespace Veritabanı_proje.Controllers
{
    [AuthorizeRoles(Role.EvSahibi)]
    public class EvSahibiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvSahibiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // Kullanıcıyı getir
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            // Evleri ayrı bir sorguda getir
            var houses = await _context.TinyHouses
                .Where(h => h.OwnerId == userId)
                .Select(h => new TinyHouse
                {
                    Id = h.Id,
                    Name = h.Name,
                    Description = h.Description,
                    Price = h.Price,
                    Location = h.Location,
                    SquareMeters = h.SquareMeters,
                    IsActive = h.IsActive,
                    OwnerId = h.OwnerId,
                    CreatedAt = h.CreatedAt
                })
                .ToListAsync();

            // Kullanıcının sahip olduğu evlerin ID'lerini al
            var ownedHouseIds = await _context.TinyHouses
                .Where(h => h.OwnerId == userId)
                .Select(h => h.Id)
                .ToListAsync();

            // O evlere ait aktif rezervasyonları çek
            var aktifRezervasyonlar = await _context.Reservations
                .Where(r => ownedHouseIds.Contains(r.TinyHouseId)
                    && (r.Status == "Onaylandı" || r.Status == "Beklemede")
                    && r.EndDate >= DateTime.Now)
                .ToListAsync();

            var kazancList = await _context.Reservations
                .Where(r => ownedHouseIds.Contains(r.TinyHouseId) && r.Status == "Ödeme Alındı")
                .Select(r => r.TotalPrice)
                .ToListAsync();

            var toplamKazanc = kazancList.Sum();

            var viewModel = new EvSahibiDashboardViewModel
            {
                User = user,
                OwnedHouses = houses,
                TotalHouses = houses.Count,
                TotalEarnings = toplamKazanc,
                ActiveReservations = aktifRezervasyonlar.Count
            };

            return View(viewModel);
        }

        // GET: EvSahibi/TinyHouses
        public async Task<IActionResult> TinyHouses()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                Console.WriteLine("User ID is null in TinyHouses action");
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcının evlerini getir
            var houses = await _context.TinyHouses
                .Where(t => t.OwnerId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            Console.WriteLine($"Found {houses.Count} houses for user ID: {userId}");
            foreach (var house in houses)
            {
                Console.WriteLine($"House ID: {house.Id}, Name: {house.Name}, Owner ID: {house.OwnerId}");
            }

            return View(houses);
        }

        [HttpGet]
        public IActionResult CreateTinyHouse()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTinyHouse(TinyHouse house)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                house.OwnerId = userId.Value;
                house.IsActive = true;
                house.CreatedAt = DateTime.Now;
                house.Beds = house.Beds == 0 ? 1 : house.Beds;
                house.Bathrooms = house.Bathrooms == 0 ? 1 : house.Bathrooms;

                _context.Add(house);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(house);
        }

        // GET: EvSahibi/Reservations
        public async Task<IActionResult> Reservations()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var reservations = await _context.Reservations
                .Include(r => r.TinyHouse)
                .Include(r => r.User)
                .Where(r => r.TinyHouse.OwnerId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reservations);
        }

        // POST: EvSahibi/UpdateReservationStatus
        [HttpPost]
        public async Task<IActionResult> UpdateReservationStatus(int reservationId, string status)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var reservation = await _context.Reservations
                .Include(r => r.TinyHouse)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.TinyHouse.OwnerId == userId);

            if (reservation == null) return NotFound();

            reservation.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyon durumu güncellendi.";
            return RedirectToAction("Reservations");
        }

        // GET: EvSahibi/Reviews
        public async Task<IActionResult> Reviews()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var reviews = await _context.Reviews
                .Include(r => r.TinyHouse)
                .Include(r => r.User)
                .Where(r => r.TinyHouse.OwnerId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reviews);
        }

        // GET: EvSahibi/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tinyHouse = await _context.TinyHouses.FindAsync(id);
            if (tinyHouse == null) return NotFound();

            return View(tinyHouse);
        }

        // POST: EvSahibi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TinyHouse house)
        {
            if (id != house.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(house);
                await _context.SaveChangesAsync();
                return RedirectToAction("TinyHouses");
            }
            return View(house);
        }

        // POST: EvSahibi/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var house = await _context.TinyHouses
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId);

            if (house == null) return NotFound();

            _context.TinyHouses.Remove(house);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TinyHouses));
        }

        private bool TinyHouseExists(int id)
        {
            return (_context.TinyHouses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
} 