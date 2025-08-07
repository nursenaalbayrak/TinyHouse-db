using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Veritabanı_proje.Data;
using Veritabanı_proje.Models;

namespace Veritabanı_proje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var houses = await _context.TinyHouses
                    .Include(h => h.Owner)
                    .Where(h => h.IsActive)
                    .ToListAsync();

                return View(houses ?? new List<TinyHouse>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ana sayfa yüklenirken hata oluştu");
                return View(new List<TinyHouse>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
