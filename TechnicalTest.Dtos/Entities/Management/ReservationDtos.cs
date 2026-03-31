using TechnicalTest.Dtos.Entities.Users;

namespace TechnicalTest.Dtos.Entities.Management
{
    // Reservation DTOs
    public class CreateReservationRequest
    {
        public int IdUsuario { get; set; }
        public int IdHotel { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdHotel { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaReserva { get; set; }
        public int Estado { get; set; } // 1=Reservado, 2=Cancelado
        public string NombreHotel { get; set; } = string.Empty;
        public string MailUsuario { get; set; } = string.Empty;
    }

    public class ReservationDetailDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdHotel { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaReserva { get; set; }
        public int Estado { get; set; }
        public string EstadoDescripcion { get; set; } = string.Empty;
        public UserDto? Usuario { get; set; }
        public HotelDto? Hotel { get; set; }
    }
}
