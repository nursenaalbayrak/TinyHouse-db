using Microsoft.AspNetCore.Mvc;
using Veritabanı_proje.Data;
using Veritabanı_proje.Models;
using System.Threading.Tasks;
using System.Linq;

public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;
    public PaymentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Pay(int reservationId)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation == null) return NotFound();

        return View(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Pay(int reservationId, string paymentMethod)
    {
        var reservation = await _context.Reservations.FindAsync(reservationId);
        if (reservation == null) return NotFound();

        // Dummy ödeme işlemi
        var payment = new Payment
        {
            ReservationId = reservationId,
            Amount = reservation.TotalPrice,
            Status = "Tamamlandı",
            PaymentMethod = paymentMethod,
            CreatedAt = DateTime.Now
        };
        _context.Payments.Add(payment);
        reservation.Status = "Ödeme Alındı";
        await _context.SaveChangesAsync();

        TempData["Success"] = "Ödeme başarıyla tamamlandı!";

        // Ödeme durumu (örneğin "Beklemede" olarak)
        ViewBag.PaymentStatus = "Beklemede";
        return View();
    }
}