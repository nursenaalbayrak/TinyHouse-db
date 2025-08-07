using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Dapper;
using Veritabanı_proje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class RezervasyonController : Controller
    {
        private readonly IConfiguration _configuration;
        
        public RezervasyonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Create(int evId)
        {
            var rezervasyon = new Rezervasyon { EvId = evId };
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                rezervasyon.UserId = int.Parse(userIdClaim);
            }
            return View(rezervasyon);
        }

        [HttpPost]
        public IActionResult Create(Rezervasyon rezervasyon)
        {
            if (ModelState.IsValid)
            {
                using var connection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection"));
                
                var sql = @"INSERT INTO Rezervasyonlar 
                           (EvId, MusteriAdi, MusteriSoyadi, Telefon, BaslangicTarihi, 
                            BitisTarihi, ToplamFiyat, RezervasyonTarihi, Aktif)
                           VALUES 
                           (@EvId, @MusteriAdi, @MusteriSoyadi, @Telefon, @BaslangicTarihi, 
                            @BitisTarihi, @ToplamFiyat, @RezervasyonTarihi, @Aktif)";

                connection.Execute(sql, rezervasyon);

                return RedirectToAction("Index", "Home");
            }
            return View(rezervasyon);
        }
    }
} 