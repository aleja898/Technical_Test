using System.ComponentModel.DataAnnotations;
using TechnicalTest.ClassLibrary.Interfaces;
using TechnicalTest.ClassLibrary.Entities.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechnicalTest.ClassLibrary.Entities.Management
{
    public enum ReservationStatus
    {
        Reservado = 1,
        Cancelado = 2
    }

    public class Reservation : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Usuario")]
        public int IdUsuario { get; set; }

        public User Usuario { get; set; } = null!;

        [Required]
        [Display(Name = "Hotel")]
        public int IdHotel { get; set; }

        public Hotel Hotel { get; set; } = null!;

        [Required]
        [Display(Name = "Habitación")]
        public int IdHabitacion { get; set; }

        [Required]
        [Display(Name = "Fecha de entrada")]
        public DateTime FechaEntrada { get; set; }

        [Display(Name = "Fecha de salida")]
        public DateTime? FechaSalida { get; set; }

        [Required]
        [Display(Name = "Fecha de reserva")]
        public DateTime FechaReserva { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Estado")]
        public ReservationStatus Estado { get; set; } = ReservationStatus.Reservado;
    }
}