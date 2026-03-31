using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Backend.Data;
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

        // GET: api/hotels
        [HttpGet]
        public async Task<ActionResult<HotelDto>> GetHotels()
        {
            try
            {
                var hotels = await _context.Hotels.Where(h => h.Activo).ToListAsync();
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

        // GET: api/hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
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

        // POST: api/hotels
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

        // PUT: api/hotels/5
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

        // DELETE: api/hotels/5
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

                // Verificar si tiene reservas activas
                var hasReservations = await _context.Reservations
                    .AnyAsync(r => r.IdHotel == id && r.Estado == ReservationStatus.Reservado);
                
                if (hasReservations)
                {
                    _logger.LogWarning("No se puede eliminar hotel {HotelId} porque tiene reservas activas", id);
                    return BadRequest("No se puede eliminar el hotel porque tiene reservas activas");
                }

                _context.Hotels.Remove(hotel);
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

        // GET: api/hotels/5/availability?startDate=2024-01-01&endDate=2024-01-31
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

                // Contar reservas activas en el rango de fechas
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
