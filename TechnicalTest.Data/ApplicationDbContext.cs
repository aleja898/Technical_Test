using Microsoft.EntityFrameworkCore;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.ClassLibrary.Entities.Management;

namespace TechnicalTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Mail).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Mail).IsUnique();
                entity.Property(e => e.Direccion).HasMaxLength(300);
            });

            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Pais).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(1000);
                entity.Property(e => e.NumeroHabitaciones).IsRequired();
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Estado).IsRequired();

                entity.HasOne(r => r.Usuario)
                      .WithMany(u => u.Reservations)
                      .HasForeignKey(r => r.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);
                      
                entity.HasOne(r => r.Hotel)
                      .WithMany(h => h.Reservations)
                      .HasForeignKey(r => r.IdHotel)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
