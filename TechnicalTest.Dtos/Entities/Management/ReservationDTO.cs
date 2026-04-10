using System.ComponentModel.DataAnnotations;
using TechnicalTest.Dtos.Entities.Users;

namespace TechnicalTest.Dtos.Entities.Management
{
    public class CreateReservationRequest
    {
        [Required(ErrorMessage = "El usuario es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un usuario válido")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El hotel es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un hotel válido")]
        public int IdHotel { get; set; }

        [Required(ErrorMessage = "La habitación es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un número de habitación válido")]
        [CustomValidation(typeof(ReservationValidator), nameof(ReservationValidator.ValidateRoomNumber))]
        public int IdHabitacion { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es requerida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaEntrada { get; set; }

        public DateOnly FechaEntradaDate { get; set; }

        [Required(ErrorMessage = "La hora de entrada es requerida")]
        [Range(0, 23, ErrorMessage = "La hora debe estar entre 0 y 23")]
        public int HoraEntrada { get; set; }

        [Range(0, 59, ErrorMessage = "Los minutos deben estar entre 0 y 59")]
        public int MinutoEntrada { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? FechaSalida { get; set; }

        public DateOnly? FechaSalidaDate { get; set; }

        [Range(0, 23, ErrorMessage = "La hora debe estar entre 0 y 23")]
        public int? HoraSalida { get; set; }

        [Range(0, 59, ErrorMessage = "Los minutos deben estar entre 0 y 59")]
        public int? MinutoSalida { get; set; }

        public void CombinarFechaHora()
        {
            FechaEntrada = FechaEntradaDate.ToDateTime(TimeOnly.FromDateTime(new DateTime(1, 1, 1, HoraEntrada, MinutoEntrada, 0)));

            if (FechaSalidaDate.HasValue && HoraSalida.HasValue && MinutoSalida.HasValue)
            {
                FechaSalida = FechaSalidaDate.Value.ToDateTime(TimeOnly.FromDateTime(new DateTime(1, 1, 1, HoraSalida.Value, MinutoSalida.Value, 0)));
            }
        }
    }

    public class DateNotEarlierThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateNotEarlierThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue < comparisonValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }

    public class ReservationValidator
    {
        public static ValidationResult ValidateRoomNumber(int roomNumber, ValidationContext context)
        {
            var instance = context.ObjectInstance as CreateReservationRequest;
            if (instance == null)
                return ValidationResult.Success;

            return ValidationResult.Success;
        }
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdHotel { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
        public DateTime FechaReserva { get; set; }
        public int Estado { get; set; }
        public string NombreHotel { get; set; } = string.Empty;
        public string MailUsuario { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
    }

    public class ReservationDetailDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdHotel { get; set; }
        public int IdHabitacion { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
        public DateTime FechaReserva { get; set; }
        public int Estado { get; set; }
        public string EstadoDescripcion { get; set; } = string.Empty;
        public UserDto? Usuario { get; set; }
        public HotelDto? Hotel { get; set; }
    }
}