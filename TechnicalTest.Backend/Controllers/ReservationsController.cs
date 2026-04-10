using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Data;
using TechnicalTest.ClassLibrary.Entities.Management;
using TechnicalTest.Dtos.Entities.Management;
using TechnicalTest.Dtos.Entities.Users;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDetailDto>>> GetAllReservations()
        {
            try
            {
                var reservations = await (from r in _context.Reservations
                                     join u in _context.Users on r.IdUsuario equals u.Id
                                     join h in _context.Hotels on r.IdHotel equals h.Id
                                     select new ReservationDetailDto
                                     {
                                         Id = r.Id,
                                         IdUsuario = r.IdUsuario,
                                         IdHotel = r.IdHotel,
                                         IdHabitacion = r.IdHabitacion,
                                         FechaEntrada = r.FechaEntrada,
                                         FechaSalida = r.FechaSalida,
                                         FechaReserva = r.FechaReserva,
                                         Estado = (int)r.Estado,
                                         EstadoDescripcion = r.Estado == ReservationStatus.Reservado ? "Reservado" : "Cancelado",
                                         Usuario = new UserDto
                                         {
                                             Id = u.Id,
                                             Nombre = u.Nombre,
                                             Apellidos = u.Apellidos,
                                             Mail = u.Mail,
                                             Direccion = u.Direccion
                                         },
                                         Hotel = new HotelDto
                                         {
                                             Id = h.Id,
                                             Nombre = h.Nombre,
                                             Pais = h.Pais,
                                             Latitud = h.Latitud,
                                             Longitud = h.Longitud,
                                             Descripcion = h.Descripcion,
                                             Activo = h.Activo
                                         }
                                     })
                    .OrderByDescending(r => r.FechaReserva)
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las reservas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation([FromBody] CreateReservationRequest request)
        {
            try
            {
                request.CombinarFechaHora();
                
                var user = await _context.Users.FindAsync(request.IdUsuario);
                if (user == null)
                {
                    return NotFound($"Usuario con ID {request.IdUsuario} no encontrado");
                }

                var hotel = await _context.Hotels.FindAsync(request.IdHotel);
                if (hotel == null || !hotel.Activo)
                {
                    return NotFound($"Hotel con ID {request.IdHotel} no encontrado o inactivo");
                }

                if (request.FechaSalida.HasValue && request.FechaEntrada > request.FechaSalida.Value)
                {
                    return BadRequest("La fecha de entrada no puede ser mayor que la fecha de salida");
                }

                if (request.IdHabitacion > hotel.NumeroHabitaciones)
                {
                    return BadRequest($"El número de habitación no puede exceder {hotel.NumeroHabitaciones} para este hotel");
                }

                var existingReservations = await _context.Reservations
                    .Where(r => r.IdHotel == request.IdHotel 
                               && r.IdHabitacion == request.IdHabitacion
                               && r.Estado == ReservationStatus.Reservado)
                    .ToListAsync();

                bool hasOverlap = false;
                
                foreach (var existing in existingReservations)
                {
                    if (request.FechaSalida.HasValue)
                    {
                        if (existing.FechaSalida.HasValue)
                        {
                            if (existing.FechaEntrada < request.FechaSalida.Value && 
                                existing.FechaSalida.Value > request.FechaEntrada)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                        else
                        {
                            if (existing.FechaEntrada <= request.FechaSalida.Value)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (existing.FechaSalida.HasValue)
                        {
                            if (existing.FechaSalida.Value > request.FechaEntrada)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                        else
                        {
                            if (existing.FechaEntrada <= request.FechaEntrada)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                    }
                }

                if (hasOverlap)
                {
                    if (request.FechaSalida.HasValue)
                    {
                        return BadRequest($"La habitación {request.IdHabitacion} ya está reservada para las fechas seleccionadas. Por favor, seleccione otra habitación o fechas diferentes.");
                    }
                    else
                    {
                        return BadRequest($"La habitación {request.IdHabitacion} ya está ocupada. Por favor, seleccione otra habitación.");
                    }
                }

                if (request.FechaSalida.HasValue)
                {
                    var overlappingReservations = await _context.Reservations
                        .Where(r => r.IdHotel == request.IdHotel 
                                   && r.Estado == ReservationStatus.Reservado
                                   && r.FechaEntrada < request.FechaSalida.Value 
                                   && r.FechaSalida.HasValue
                                   && r.FechaSalida.Value > request.FechaEntrada)
                        .CountAsync();

                    if (overlappingReservations >= hotel.NumeroHabitaciones)
                    {
                        return BadRequest("No hay habitaciones disponibles para las fechas solicitadas");
                    }
                }

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

                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear reserva");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Reservation>> UpdateReservation(int id, [FromBody] CreateReservationRequest request)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Reserva con ID {id} no encontrada");
                }

                request.CombinarFechaHora();
                
                var user = await _context.Users.FindAsync(request.IdUsuario);
                if (user == null)
                {
                    return NotFound($"Usuario con ID {request.IdUsuario} no encontrado");
                }

                var hotel = await _context.Hotels.FindAsync(request.IdHotel);
                if (hotel == null || !hotel.Activo)
                {
                    return NotFound($"Hotel con ID {request.IdHotel} no encontrado o inactivo");
                }

                if (request.FechaSalida.HasValue && request.FechaEntrada > request.FechaSalida.Value)
                {
                    return BadRequest("La fecha de entrada no puede ser mayor que la fecha de salida");
                }

                if (request.IdHabitacion > hotel.NumeroHabitaciones)
                {
                    return BadRequest($"El número de habitación no puede exceder {hotel.NumeroHabitaciones} para este hotel");
                }

                // Validar habitación duplicada para todos los casos, EXCEPTO la reserva actual que se está editando
                var existingReservations = await _context.Reservations
                    .Where(r => r.IdHotel == request.IdHotel 
                               && r.IdHabitacion == request.IdHabitacion
                               && r.Estado == ReservationStatus.Reservado
                               && r.Id != id) // Excluir la reserva actual que se está editando
                    .ToListAsync();

                bool hasOverlap = false;
                
                foreach (var existing in existingReservations)
                {
                    if (request.FechaSalida.HasValue)
                    {
                        if (existing.FechaSalida.HasValue)
                        {
                            if (existing.FechaEntrada < request.FechaSalida.Value && 
                                existing.FechaSalida.Value > request.FechaEntrada)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                        else
                        {
                            if (existing.FechaEntrada <= request.FechaSalida.Value)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (existing.FechaSalida.HasValue)
                        {
                            if (existing.FechaSalida.Value > request.FechaEntrada)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                        else
                        {
                            if (existing.FechaEntrada <= request.FechaEntrada)
                            {
                                hasOverlap = true;
                                break;
                            }
                        }
                    }
                }

                if (hasOverlap)
                {
                    if (request.FechaSalida.HasValue)
                    {
                        return BadRequest($"La habitación {request.IdHabitacion} ya está reservada para las fechas seleccionadas. Por favor, seleccione otra habitación o fechas diferentes.");
                    }
                    else
                    {
                        return BadRequest($"La habitación {request.IdHabitacion} ya está ocupada. Por favor, seleccione otra habitación.");
                    }
                }

                if (request.FechaSalida.HasValue)
                {
                    var overlappingReservations = await _context.Reservations
                        .Where(r => r.IdHotel == request.IdHotel 
                                   && r.Estado == ReservationStatus.Reservado
                                   && r.FechaEntrada < request.FechaSalida.Value 
                                   && r.FechaSalida.HasValue
                                   && r.FechaSalida.Value > request.FechaEntrada
                                   && r.Id != id) // Excluir la reserva actual
                        .CountAsync();

                    if (overlappingReservations >= hotel.NumeroHabitaciones)
                    {
                        return BadRequest("No hay habitaciones disponibles para las fechas solicitadas");
                    }
                }

                // Actualizar la reserva existente
                reservation.IdUsuario = request.IdUsuario;
                reservation.IdHotel = request.IdHotel;
                reservation.IdHabitacion = request.IdHabitacion;
                reservation.FechaEntrada = request.FechaEntrada;
                reservation.FechaSalida = request.FechaSalida;

                await _context.SaveChangesAsync();

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar reserva con ID {ReservationId}", id);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    _logger.LogWarning("Reserva con ID {ReservationId} no encontrada", id);
                    return NotFound($"Reserva con ID {id} no encontrada");
                }

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reserva {ReservationId} eliminada exitosamente", id);
                return Ok("Reserva eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar reserva con ID {ReservationId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetActiveReservationsByHotel(
            int hotelId, DateTime startDate, DateTime endDate)
        {
            try
            {

                var hotel = await _context.Hotels
                    .FirstOrDefaultAsync(h => h.Id == hotelId && h.Activo);

                if (hotel == null)
                {
                    _logger.LogWarning("Hotel con ID {HotelId} no encontrado o inactivo", hotelId);
                    return NotFound($"Hotel con ID {hotelId} no encontrado o inactivo");
                }

                var reservations = await _context.Reservations
                    .Where(r => r.IdHotel == hotelId 
                               && r.Estado == ReservationStatus.Reservado
                               && r.FechaEntrada < endDate 
                               && r.FechaSalida > startDate)
                    .ToListAsync();


                var userIds = reservations.Select(r => r.IdUsuario).Distinct().ToList();
                var users = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u);


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

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Usuario)
                    .Include(r => r.Hotel)
                    .FirstOrDefaultAsync(r => r.Id == id);
                
                if (reservation == null)
                {
                    return NotFound();
                }

                var reservationDto = new ReservationDto
                {
                    Id = reservation.Id,
                    IdUsuario = reservation.IdUsuario,
                    IdHotel = reservation.IdHotel,
                    IdHabitacion = reservation.IdHabitacion,
                    FechaEntrada = reservation.FechaEntrada,
                    FechaSalida = reservation.FechaSalida,
                    FechaReserva = reservation.FechaReserva,
                    Estado = (int)reservation.Estado,
                    NombreHotel = reservation.Hotel?.Nombre ?? "N/A",
                    MailUsuario = reservation.Usuario?.Mail ?? "N/A",
                    NombreUsuario = reservation.Usuario?.Nombre + " " + reservation.Usuario?.Apellidos ?? "N/A"
                };

                return Ok(reservationDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reserva con ID {ReservationId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}/checkout-date")]
        public async Task<IActionResult> UpdateCheckoutDate(int id, [FromBody] DateTime checkoutDate)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound($"Reserva con ID {id} no encontrada");
                }

                if (checkoutDate <= reservation.FechaEntrada)
                {
                    return BadRequest("La fecha de salida debe ser posterior a la fecha de entrada");
                }

                reservation.FechaSalida = checkoutDate;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Fecha de salida actualizada para reserva {ReservationId}", id);
                return Ok(new { message = "Fecha de salida actualizada correctamente", checkoutDate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar fecha de salida para reserva {ReservationId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
