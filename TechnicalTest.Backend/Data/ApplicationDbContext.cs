using Microsoft.EntityFrameworkCore;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.ClassLibrary.Entities.Management;

namespace TechnicalTest.Backend.Data
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

            // Configuración de User
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

            // Configuración de Hotel
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Pais).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(1000);
                entity.Property(e => e.NumeroHabitaciones).IsRequired();
            });

            // Configuración de Reservation
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Estado).IsRequired();

                // Foreign Keys sin navegación
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Hotel>()
                      .WithMany()
                      .HasForeignKey(e => e.IdHotel)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
