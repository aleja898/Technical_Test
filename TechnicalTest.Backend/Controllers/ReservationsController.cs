using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Backend.Data;
using TechnicalTest.ClassLibrary.Entities.Management;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.Dtos.Entities.Management;

namespace TechnicalTest.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(ApplicationDbContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation([FromBody] CreateReservationRequest request)
        {
            try
            {
                // Validar que el usuario y hotel existan
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.IdUsuario);
                var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == request.IdHotel && h.Activo);

                if (!userExists)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", request.IdUsuario);
                    return BadRequest($"Usuario con ID {request.IdUsuario} no encontrado");
                }

                if (!hotelExists)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado o inactivo", request.IdHotel);
                    return BadRequest($"Hotel con ID {request.IdHotel} no encontrado o inactivo");
                }

                // Validar fechas
                if (request.FechaEntrada >= request.FechaSalida)
                {
                    _logger.LogWarning("Fecha de entrada {CheckIn} debe ser menor que fecha de salida {CheckOut}", 
                        request.FechaEntrada, request.FechaSalida);
                    return BadRequest("La fecha de entrada debe ser menor que la fecha de salida");
                }

                // Validar disponibilidad (no overbooking)
                var hotel = await _context.Hotels.FindAsync(request.IdHotel);
                var overlappingReservations = await _context.Reservations
                    .Where(r => r.IdHotel == request.IdHotel 
                               && r.Estado == ReservationStatus.Reservado
                               && r.FechaEntrada < request.FechaSalida 
                               && r.FechaSalida > request.FechaEntrada)
                    .CountAsync();

                if (overlappingReservations >= hotel.NumeroHabitaciones)
                {
                    _logger.LogWarning("No hay habitaciones disponibles para el hotel {HotelId} en las fechas solicitadas", 
                        request.IdHotel);
                    return BadRequest("No hay habitaciones disponibles para las fechas solicitadas");
                }

                // Crear reserva
                var reservation = new Reservation
                {
                    IdUsuario = request.IdUsuario,
                    IdHotel = request.IdHotel,
                    IdHabitacion = request.IdHabitacion,
                    FechaEntrada = request.FechaEntrada,
                    FechaSalida = request.FechaSalida,
                    FechaReserva = DateTime.Now,
                    Estado = ReservationStatus.Reservado
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reserva creada exitosamente con ID {ReservationId}", reservation.Id);
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, new ReservationDto
                {
                    Id = reservation.Id,
                    IdUsuario = reservation.IdUsuario,
                    IdHotel = reservation.IdHotel,
                    IdHabitacion = reservation.IdHabitacion,
                    FechaEntrada = reservation.FechaEntrada,
                    FechaSalida = reservation.FechaSalida,
                    FechaReserva = reservation.FechaReserva,
                    Estado = (int)reservation.Estado,
                    NombreHotel = hotel.Nombre,
                    MailUsuario = "N/A" // Se cargaría con consulta adicional si se necesita
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reserva");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning("Reserva con ID {ReservationId} no encontrada", id);
                    return NotFound($"Reserva con ID {id} no encontrada");
                }

                if (reservation.Estado == ReservationStatus.Cancelado)
                {
                    _logger.LogWarning("Reserva con ID {ReservationId} ya está cancelada", id);
                    return BadRequest("La reserva ya está cancelada");
                }

                reservation.Estado = ReservationStatus.Cancelado;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reserva {ReservationId} cancelada exitosamente", id);
                return Ok("Reserva cancelada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar reserva con ID {ReservationId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/reservations/hotel/5?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetActiveReservationsByHotel(
            int hotelId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Validar que el hotel exista y esté activo
                var hotel = await _context.Hotels
                    .FirstOrDefaultAsync(h => h.Id == hotelId && h.Activo);

                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado o inactivo", hotelId);
                    return NotFound($"Hotel con ID {hotelId} no encontrado o inactivo");
                }

                // Obtener reservas activas que overlapean con el rango de fechas
                var reservations = await _context.Reservations
                    .Where(r => r.IdHotel == hotelId 
                               && r.Estado == ReservationStatus.Reservado
                               && r.FechaEntrada < endDate 
                               && r.FechaSalida > startDate)
                    .ToListAsync();

                // Cargar información de usuarios
                var userIds = reservations.Select(r => r.IdUsuario).Distinct().ToList();
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u);

                // Mapear a DTOs
                var result = reservations.Select(r => new ReservationDto
                {
                    Id = r.Id,
                    IdUsuario = r.IdUsuario,
                    IdHotel = r.IdHotel,
                    IdHabitacion = r.IdHabitacion,
                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,
                    FechaReserva = r.FechaReserva,
                    Estado = (int)r.Estado,
                    NombreHotel = hotel.Nombre,
                    MailUsuario = users.ContainsKey(r.IdUsuario) ? users[r.IdUsuario].Mail : "N/A"
                }).ToList();

                _logger.LogInformation("Se encontraron {Count} reservas activas para el hotel {HotelId}", 
                    result.Count, hotelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar reservas del hotel {HotelId}", hotelId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound();
                }
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reserva con ID {ReservationId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
