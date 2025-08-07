namespace Veritabanı_proje.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalTinyHouses { get; set; }
        public int TotalReservations { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ToplamKazanc { get; set; }
    }
} 