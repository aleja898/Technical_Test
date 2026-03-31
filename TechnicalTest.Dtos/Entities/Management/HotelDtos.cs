namespace TechnicalTest.Dtos.Entities.Management
{
    public class CreateHotelRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int NumeroHabitaciones { get; set; }
    }

    public class UpdateHotelRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int NumeroHabitaciones { get; set; }
    }

    public class HotelDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int NumeroHabitaciones { get; set; }
    }

    public class HotelAvailabilityDto
    {
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
