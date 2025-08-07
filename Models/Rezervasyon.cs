namespace Veritabanı_proje.Models
{
    public class Rezervasyon
    {
        public int Id { get; set; }
        public string MusteriAdi { get; set; }
        public string MusteriSoyadi { get; set; }
        public string Telefon { get; set; }
        public int EvId { get; set; }
        public int UserId { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal ToplamFiyat { get; set; }
        public DateTime RezervasyonTarihi { get; set; }
        public bool Aktif { get; set; }

        // Navigation property
        public TinyHouse Ev { get; set; }
        public User User { get; set; }
    }
} 