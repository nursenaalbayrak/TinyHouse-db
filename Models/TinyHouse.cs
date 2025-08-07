using System.ComponentModel.DataAnnotations;

namespace Veritabanı_proje.Models
{
    public class TinyHouse
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ev başlığı zorunludur")]
        [Display(Name = "Başlık")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama zorunludur")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Display(Name = "Gecelik Fiyat")]
        [Range(1, 100000, ErrorMessage = "Fiyat 1-100000 arasında olmalıdır")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Konum zorunludur")]
        [Display(Name = "Konum")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alan zorunludur")]
        [Display(Name = "Alan (m²)")]
        [Range(1, 1000, ErrorMessage = "Alan 1-1000 m² arasında olmalıdır")]
        public int SquareMeters { get; set; }

        [Required(ErrorMessage = "Yatak sayısı zorunludur")]
        [Display(Name = "Yatak Sayısı")]
        [Range(1, 10, ErrorMessage = "Yatak sayısı 1-10 arasında olmalıdır")]
        public int Beds { get; set; } = 1;

        [Required(ErrorMessage = "Banyo sayısı zorunludur")]
        [Display(Name = "Banyo Sayısı")]
        [Range(1, 5, ErrorMessage = "Banyo sayısı 1-5 arasında olmalıdır")]
        public int Bathrooms { get; set; }

        //[Display(Name = "Özellikler")]
        //public string? Amenities { get; set; } // JSON formatında saklanacak: ["Wifi", "TV", "Klima"]

        [Display(Name = "Fotoğraflar")]
        public string? Photos { get; set; } // JSON formatında saklanacak: ["foto1.jpg", "foto2.jpg"]

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        public List<Reservation>? Reservations { get; set; }
        public List<Review>? Reviews { get; set; }
    }
} 