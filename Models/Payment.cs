using System;
using System.ComponentModel.DataAnnotations;

namespace Veritabanı_proje.Models
{
    public class Payment
    {
        public int Id { get; set; }
        
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public bool IsPaid { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;
        
        public DateTime? PaidAt { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; }
    }
}
