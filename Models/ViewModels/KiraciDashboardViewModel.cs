using Veritabanı_proje.Models;
using System.Collections.Generic;

namespace Veritabanı_proje.Models.ViewModels
{
    public class KiraciDashboardViewModel
    {
        public User? User { get; set; }
        public List<Reservation> ActiveReservations { get; set; } = new();
        public List<Reservation> PastReservations { get; set; } = new();
        public List<TinyHouse> AvailableHouses { get; set; } = new();
    }
} 