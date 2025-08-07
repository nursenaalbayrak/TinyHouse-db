using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Veritabanı_proje.Models;
using Veritabanı_proje.Models.ViewModels;
using Veritabanı_proje.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Veritabanı_proje.Attributes;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;

namespace Veritabanı_proje.Controllers
{
    [AuthorizeRoles(Role.Kiraci)]
    public class KiraciController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KiraciController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            var viewModel = new KiraciDashboardViewModel
            {
                User = user,
                ActiveReservations = await _context.Reservations
                    .Include(r => r.TinyHouse)
                    .Where(r => r.UserId == userId && r.EndDate >= DateTime.Now && r.Status != "İptalEdildi")
                    .ToListAsync(),
                PastReservations = await _context.Reservations
                    .Include(r => r.TinyHouse)
                    .Where(r => r.UserId == userId && r.EndDate < DateTime.Now)
                    .ToListAsync(),
                AvailableHouses = await _context.TinyHouses
                    .Where(h => h.IsActive)
                    .Select(h => new TinyHouse
                    {
                        Id = h.Id,
                        Name = h.Name,
                        Description = h.Description,
                        Price = h.Price,
                        Location = h.Location,
                        SquareMeters = h.SquareMeters,
                        IsActive = h.IsActive,
                        OwnerId = h.OwnerId
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // Ev detaylarını görüntüleme
        public async Task<IActionResult> HouseDetails(int id)
        {
            var house = await _context.TinyHouses
                .Include(h => h.Owner)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }

        // Rezervasyon yapma
        [HttpPost]
        public async Task<IActionResult> MakeReservation(int houseId, DateTime startDate, DateTime endDate)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var house = await _context.TinyHouses.FindAsync(houseId);
            if (house == null)
            {
                return NotFound();
            }

            // Tarih çakışması kontrolü
            var hasConflict = await _context.Reservations
                .AnyAsync(r => r.TinyHouseId == houseId &&
                              r.Status != "İptalEdildi" &&
                              (
                                  (startDate >= r.StartDate && startDate < r.EndDate) ||
                                  (endDate > r.StartDate && endDate <= r.EndDate) ||
                                  (startDate <= r.StartDate && endDate >= r.EndDate)
                              ));

            if (hasConflict)
            {
                TempData["Error"] = "Seçilen tarihlerde bu ev için zaten bir rezervasyon var.";
                return RedirectToAction("HouseDetails", new { id = houseId });
            }

            if (startDate >= endDate)
            {
                TempData["Error"] = "Çıkış tarihi, giriş tarihinden sonra olmalı!";
                return RedirectToAction("HouseDetails", new { id = houseId });
            }

            var totalDays = (endDate - startDate).Days;
            var totalPrice = house.Price * totalDays;

            var reservation = new Reservation
            {
                TinyHouseId = houseId,
                UserId = userId.Value,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = totalPrice,
                Status = "Beklemede",
                CreatedAt = DateTime.Now
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyon talebiniz alınmıştır.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPhotos(List<IFormFile> photos, int houseId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var house = await _context.TinyHouses.FindAsync(houseId);
            if (house == null)
            {
                return NotFound();
            }

            if (photos != null && photos.Count > 0)
            {
                var photoPaths = new List<string>();
                foreach (var photo in photos)
                {
                    var fileName = Path.GetFileName(photo.FileName);
                    var filePath = Path.Combine("wwwroot/images", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }
                    photoPaths.Add("/images/" + fileName);
                }
                house.Photos = JsonSerializer.Serialize(photoPaths);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Fotoğraflar başarıyla yüklendi.";
            return RedirectToAction("HouseDetails", new { id = houseId });
        }

        // Aktif rezervasyonlar
        public async Task<IActionResult> AktifRezervasyonlar()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var rezervasyonlar = await _context.Reservations
                .Include(r => r.TinyHouse)
                .Where(r => r.UserId == userId && r.EndDate >= DateTime.Now && r.Status != "İptalEdildi")
                .ToListAsync();

            return View(rezervasyonlar);
        }

        // Geçmiş rezervasyonlar
        public async Task<IActionResult> GecmisRezervasyonlar()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var rezervasyonlar = await _context.Reservations
                .Include(r => r.TinyHouse)
                .Where(r => r.UserId == userId && r.EndDate < DateTime.Now)
                .ToListAsync();

            return View(rezervasyonlar);
        }

        // GET: Yorum ekleme formu
        [HttpGet]
        public async Task<IActionResult> AddReview(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.TinyHouse)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.EndDate > DateTime.Now)
                return NotFound(); // Sadece tamamlanmış rezervasyonlar

            var model = new Review
            {
                TinyHouseId = reservation.TinyHouseId,
                UserId = reservation.UserId
            };
            return View(model);
        }

        // POST: Yorum kaydet
        [HttpPost]
        public async Task<IActionResult> AddReview(Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreatedAt = DateTime.Now;
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Yorumunuz kaydedildi!";
                return RedirectToAction("Index", "Kiraci");
            }
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> CancelReservation(int reservationId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.UserId == userId);

            if (reservation == null) return NotFound();

            reservation.Status = "İptalEdildi";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Rezervasyon başarıyla iptal edildi.";
            return RedirectToAction("Index");
        }

        // Test için geçmiş rezervasyon oluşturma
        [HttpPost]
        public async Task<IActionResult> CreateTestReservation(int houseId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var house = await _context.TinyHouses.FindAsync(houseId);
            if (house == null) return NotFound();

            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now.AddDays(-28);
            var totalDays = (endDate - startDate).Days;
            var totalPrice = house.Price * totalDays;

            var reservation = new Reservation
            {
                TinyHouseId = houseId,
                UserId = userId.Value,
                StartDate = startDate,
                EndDate = endDate,
                TotalPrice = totalPrice,
                Status = "Tamamlandı",
                CreatedAt = DateTime.Now.AddDays(-31)
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Test rezervasyonu başarıyla oluşturuldu.";
            return RedirectToAction("GecmisRezervasyonlar");
        }
    }
} 