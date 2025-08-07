using System.ComponentModel.DataAnnotations;

namespace Veritabanı_proje.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        
        public int TinyHouseId { get; set; }
        public TinyHouse? TinyHouse { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public decimal TotalPrice { get; set; }
        
        [Required]
        public string Status { get; set; } = "Pending";
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
