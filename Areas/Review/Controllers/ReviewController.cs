using Microsoft.AspNetCore.Mvc;
using Veritabanı_proje.Data;
using Veritabanı_proje.Models;
using System.Threading.Tasks;

namespace Veritabanı_proje.Areas.Review.Controllers
{
    [Area("Review")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Review/AddReview?reservationId=10
        public IActionResult AddReview(int reservationId)
        {
            var review = new Veritabanı_proje.Models.Review { ReservationId = reservationId };
            return View(review);
        }

        // POST: Review/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(Veritabanı_proje.Models.Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            return View(review);
        }
    }
} 