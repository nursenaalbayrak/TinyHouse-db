using System.ComponentModel.DataAnnotations;

namespace Veritabanı_proje.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        public int TinyHouseId { get; set; }
        public TinyHouse? TinyHouse { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        public string Comment { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
       public int ReservationId { get; set; }
    }
}
