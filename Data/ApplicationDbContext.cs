using Microsoft.EntityFrameworkCore;
using Veritabanı_proje.Models;

namespace Veritabanı_proje.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TinyHouse> TinyHouses { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity konfigürasyonu
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                
                // Role özel konfigürasyonu
                entity.Property(e => e.Role)
                    .HasConversion<int>()  // Enum'ı int olarak sakla
                    .IsRequired();
                
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Phone).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // TinyHouse entity konfigürasyonu
            modelBuilder.Entity<TinyHouse>(entity =>
            {
                entity.ToTable("TinyHouses");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Price).IsRequired().HasColumnType("DECIMAL");
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.SquareMeters).IsRequired();
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Owner ilişkisi
                entity.HasOne(e => e.Owner)
                      .WithMany(e => e.OwnedHouses)
                      .HasForeignKey(e => e.OwnerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Reservation entity konfigürasyonu
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservations");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StartDate).IsRequired();
                entity.Property(e => e.EndDate).IsRequired();
                entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("DECIMAL");
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // İlişkiler
                entity.HasOne(e => e.TinyHouse)
                      .WithMany(e => e.Reservations)
                      .HasForeignKey(e => e.TinyHouseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany(e => e.Reservations)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Review entity konfigürasyonu
            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rating).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // İlişkiler
                entity.HasOne(e => e.TinyHouse)
                      .WithMany(e => e.Reviews)
                      .HasForeignKey(e => e.TinyHouseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                      .WithMany(e => e.Reviews)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // User - TinyHouse ilişkisi (Owner)
            modelBuilder.Entity<TinyHouse>()
                .HasOne(t => t.Owner)
                .WithMany(u => u.OwnedHouses)
                .HasForeignKey(t => t.OwnerId);

            // User - Reservation ilişkisi
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId);

            // TinyHouse - Reservation ilişkisi
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.TinyHouse)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TinyHouseId);

            // User - Review ilişkisi
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            // TinyHouse - Review ilişkisi
            modelBuilder.Entity<Review>()
                .HasOne(r => r.TinyHouse)
                .WithMany(t => t.Reviews)
                .HasForeignKey(r => r.TinyHouseId);
        }
    }
} 