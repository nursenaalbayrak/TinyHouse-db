using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Veritabanı_proje.Data;
using Veritabanı_proje.Models;
using System.Linq;
using System.Collections.Generic;

namespace Veritabanı_proje.Controllers
{
    public class TinyHousesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TinyHousesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var houses = await _context.TinyHouses
                .Include(h => h.Owner)
                .Where(h => h.IsActive)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return View(houses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinyHouse = await _context.TinyHouses
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tinyHouse == null)
            {
                return NotFound();
            }

            return View(tinyHouse);
        }

        // ... diğer action'lar ...
    }
}