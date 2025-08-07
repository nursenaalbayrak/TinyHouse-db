using Veritabanı_proje.Models;
using System.Collections.Generic;

namespace Veritabanı_proje.Models.ViewModels
{
    public class EvSahibiDashboardViewModel
    {
        public User? User { get; set; }
        public List<TinyHouse> OwnedHouses { get; set; } = new();
        public int TotalHouses { get; set; }
        public decimal TotalEarnings { get; set; }
        public int ActiveReservations { get; set; }
    }
}