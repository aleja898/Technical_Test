using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Data;
using TechnicalTest.ClassLibrary.Entities.Management;
using TechnicalTest.Dtos.Entities.Management;

namespace TechnicalTest.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(ApplicationDbContext context, ILogger<HotelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("countries")]
        public ActionResult<IEnumerable<string>> GetCountries()
        {
            try
            {
                var countries = new List<string>
                {
                    "Afganistán", "Albania", "Alemania", "Andorra", "Angola", "Antigua y Barbuda", "Arabia Saudita", "Argelia", "Argentina", "Armenia",
                    "Australia", "Austria", "Azerbaiyán", "Bahamas", "Bangladés", "Barbados", "Baréin", "Bélgica", "Belice", "Benín",
                    "Bielorrusia", "Birmania", "Bolivia", "Bosnia y Herzegovina", "Botsuana", "Brasil", "Brunéi", "Bulgaria", "Burkina Faso", "Burundi",
                    "Bután", "Cabo Verde", "Camboya", "Camerún", "Canadá", "Catar", "Chad", "Chile", "China", "Chipre",
                    "Ciudad del Vaticano", "Colombia", "Comoras", "Congo", "Corea del Norte", "Corea del Sur", "Costa de Marfil", "Costa Rica", "Croacia", "Cuba",
                    "Dinamarca", "Dominica", "Ecuador", "Egipto", "El Salvador", "Emiratos Árabes Unidos", "Eritrea", "Eslovaquia", "Eslovenia", "España",
                    "Estados Unidos", "Estonia", "Etiopía", "Filipinas", "Finlandia", "Fiyi", "Francia", "Gabón", "Gambia", "Georgia",
                    "Ghana", "Granada", "Grecia", "Guatemala", "Guinea", "Guinea Ecuatorial", "Guinea-Bisáu", "Guyana", "Haití", "Honduras",
                    "Hungría", "India", "Indonesia", "Irak", "Irán", "Irlanda", "Islandia", "Islas Marshall", "Islas Salomón", "Israel",
                    "Italia", "Jamaica", "Japón", "Jordania", "Kazajistán", "Kenia", "Kirguistán", "Kiribati", "Kuwait", "Laos",
                    "Lesoto", "Letonia", "Líbano", "Liberia", "Libia", "Liechtenstein", "Lituania", "Luxemburgo", "Macedonia del Norte", "Madagascar",
                    "Malasia", "Malaui", "Maldivas", "Mali", "Malta", "Marruecos", "Mauricio", "Mauritania", "México", "Micronesia",
                    "Moldavia", "Mónaco", "Mongolia", "Montenegro", "Mozambique", "Namibia", "Nauru", "Nepal", "Nicaragua", "Níger",
                    "Nigeria", "Noruega", "Nueva Zelanda", "Omán", "Países Bajos", "Pakistán", "Palaos", "Panamá", "Papúa Nueva Guinea", "Paraguay",
                    "Perú", "Polonia", "Portugal", "Reino Unido", "República Centroafricana", "República Checa", "República del Congo", "República Democrática del Congo", "República Dominicana", "República Eslovaca",
                    "República Sudafricana", "Ruanda", "Rumanía", "Rusia", "Samoa", "San Cristóbal y Nieves", "San Marino", "Santa Lucía", "Santo Tomé y Príncipe", "Senegal",
                    "Serbia", "Seychelles", "Sierra Leona", "Singapur", "Siria", "Somalia", "Sri Lanka", "Suazilandia", "Sudán", "Sudán del Sur",
                    "Suecia", "Suiza", "Surinam", "Tailandia", "Tanzania", "Tayikistán", "Timor Oriental", "Togo", "Tonga", "Trinidad y Tobago",
                    "Túnez", "Turkmenistán", "Turquía", "Tuvalu", "Ucrania", "Uganda", "Uruguay", "Uzbekistán", "Vanuatu", "Venezuela",
                    "Vietnam", "Yemen", "Yibuti", "Zambia", "Zimbabue"
                };

                countries.Sort();
                _logger.LogInformation("Se consultaron {Count} países", countries.Count);
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener países");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet]
        public async Task<ActionResult<HotelDto>> GetHotels()
        {
            try
            {
                var hotels = await _context.Hotels.ToListAsync(); 
                var hotelDtos = hotels.Select(h => new HotelDto
                {
                    Id = h.Id,
                    Nombre = h.Nombre,
                    Pais = h.Pais,
                    Latitud = h.Latitud,
                    Longitud = h.Longitud,
                    Descripcion = h.Descripcion,
                    Activo = h.Activo,
                    NumeroHabitaciones = h.NumeroHabitaciones
                }).ToList();
                _logger.LogInformation("Se consultaron {Count} hoteles activos", hotelDtos.Count);
                return Ok(hotelDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar hoteles");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            try
            {
                var hotel = await _context.Hotels
                    .FirstOrDefaultAsync(h => h.Id == id); 
                
                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado", id);
                    return NotFound($"Hotel con ID {id} no encontrado");
                }
                return Ok(new HotelDto
                {
                    Id = hotel.Id,
                    Nombre = hotel.Nombre,
                    Pais = hotel.Pais,
                    Latitud = hotel.Latitud,
                    Longitud = hotel.Longitud,
                    Descripcion = hotel.Descripcion,
                    Activo = hotel.Activo,
                    NumeroHabitaciones = hotel.NumeroHabitaciones
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener hotel con ID {HotelId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}/reservations")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetHotelReservations(int id)
        {
            try
            {
                var hotel = await _context.Hotels.FindAsync(id);
                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado", id);
                    return NotFound($"Hotel con ID {id} no encontrado");
                }

                var reservations = await _context.Reservations
                    .Where(r => r.IdHotel == id)
                    .Include(r => r.Usuario)
                    .Include(r => r.Hotel)
                    .OrderByDescending(r => r.FechaReserva)
                    .ToListAsync();

                var reservationDtos = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    IdUsuario = r.IdUsuario,
                    IdHotel = r.IdHotel,
                    IdHabitacion = r.IdHabitacion,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,
                    FechaReserva = r.FechaReserva,
                    Estado = (int)r.Estado,
                    NombreHotel = r.Hotel.Nombre,
                    MailUsuario = r.Usuario.Mail,
                    NombreUsuario = r.Usuario.Nombre + " " + r.Usuario.Apellidos
                });

                return Ok(reservationDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas del hotel con ID {HotelId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<HotelDto>> CreateHotel([FromBody] CreateHotelRequest request)
        {
            try
            {
                var hotel = new Hotel
                {
                    Nombre = request.Nombre,
                    Pais = request.Pais,
                    Latitud = request.Latitud,
                    Longitud = request.Longitud,
                    Descripcion = request.Descripcion,
                    Activo = true,
                    NumeroHabitaciones = request.NumeroHabitaciones
                };

                _context.Hotels.Add(hotel);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Hotel creado exitosamente con ID {HotelId}", hotel.Id);
                return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, new HotelDto
                {
                    Id = hotel.Id,
                    Nombre = hotel.Nombre,
                    Pais = hotel.Pais,
                    Latitud = hotel.Latitud,
                    Longitud = hotel.Longitud,
                    Descripcion = hotel.Descripcion,
                    Activo = hotel.Activo,
                    NumeroHabitaciones = hotel.NumeroHabitaciones
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear hotel");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelRequest request)
        {
            try
            {
                var hotel = await _context.Hotels.FindAsync(id);
                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado", id);
                    return NotFound($"Hotel con ID {id} no encontrado");
                }

                hotel.Nombre = request.Nombre;
                hotel.Pais = request.Pais;
                hotel.Latitud = request.Latitud;
                hotel.Longitud = request.Longitud;
                hotel.Descripcion = request.Descripcion;
                hotel.Activo = request.Activo;
                hotel.NumeroHabitaciones = request.NumeroHabitaciones;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Hotel {HotelId} actualizado exitosamente", id);
                return Ok("Hotel actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar hotel con ID {HotelId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                var hotel = await _context.Hotels.FindAsync(id);
                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado", id);
                    return NotFound($"Hotel con ID {id} no encontrado");
                }

                var activeReservations = await _context.Reservations
                    .Where(r => r.IdHotel == id && r.Estado == ReservationStatus.Reservado)
                    .ToListAsync();
                
                if (activeReservations.Any())
                {
                    var reservationCount = activeReservations.Count;
                    var hotelName = hotel.Nombre;
                    
                    _logger.LogWarning("No se puede eliminar hotel {HotelId} '{HotelName}' porque tiene {Count} reservas activas", 
                        id, hotelName, reservationCount);
                    
                    return BadRequest($"No se puede eliminar el hotel '{hotelName}' porque tiene {reservationCount} reserva(s) activa(s). " +
                        $"Por favor, cancele o elimine las reservas primero antes de eliminar el hotel.");
                }

                _context.Hotels.Remove(hotel!);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Hotel {HotelId} eliminado exitosamente", id);
                return Ok("Hotel eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar hotel con ID {HotelId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}/availability")]
        public async Task<ActionResult<HotelAvailabilityDto>> GetHotelAvailability(int id, DateTime startDate, DateTime endDate)
        {
            try
            {
                var hotel = await _context.Hotels
                    .FirstOrDefaultAsync(h => h.Id == id && h.Activo);

                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado o inactivo", id);
                    return NotFound($"Hotel con ID {id} no encontrado o inactivo");
                }

                var activeReservations = await _context.Reservations
                    .Where(r => r.IdHotel == id 
                               && r.Estado == ReservationStatus.Reservado
                               && r.FechaEntrada < endDate 
                               && r.FechaSalida > startDate)
                    .CountAsync();

                var availability = new HotelAvailabilityDto
                {
                    HotelId = hotel.Id,
                    HotelName = hotel.Nombre,
                    TotalRooms = hotel.NumeroHabitaciones,
                    OccupiedRooms = activeReservations,
                    AvailableRooms = hotel.NumeroHabitaciones - activeReservations,
                    StartDate = startDate,
                    EndDate = endDate
                };

                _logger.LogInformation("Disponibilidad consultada para hotel {HotelId}: {Available}/{Total} habitaciones disponibles", 
                    id, availability.AvailableRooms, availability.TotalRooms);

                return Ok(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar disponibilidad del hotel {HotelId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
