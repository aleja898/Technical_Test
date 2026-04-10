using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechnicalTest.Data;
using TechnicalTest.ClassLibrary.Entities.Users;
using TechnicalTest.ClassLibrary.Entities.Management;
using TechnicalTest.Dtos.Entities.Users;
using TechnicalTest.Dtos.Entities.Management;

namespace TechnicalTest.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellidos = u.Apellidos,
                    Mail = u.Mail,
                    Direccion = u.Direccion
                }).ToList();
                _logger.LogInformation("Se consultaron {Count} usuarios", userDtos.Count);
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar usuarios");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
                    return NotFound($"Usuario con ID {id} no encontrado");
                }
                return Ok(new UserDto
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    Mail = user.Mail,
                    Direccion = user.Direccion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {UserId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}/reservations")]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetUserReservations(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                var reservations = await _context.Reservations
                    .Where(r => r.IdUsuario == id)
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
                _logger.LogError(ex, "Error al obtener reservas del usuario con ID {UserId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Mail == request.Mail);
                
                if (existingUser != null)
                {
                    _logger.LogWarning("Email {Email} ya está registrado", request.Mail);
                    return BadRequest($"El email {request.Mail} ya está registrado");
                }

                var user = new User
                {
                    Nombre = request.Nombre,
                    Apellidos = request.Apellidos,
                    Mail = request.Mail,
                    Direccion = request.Direccion
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario creado exitosamente con ID {UserId}", user.Id);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    Mail = user.Mail,
                    Direccion = user.Direccion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                if (request.Mail != user.Mail)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Mail == request.Mail && u.Id != id);
                    
                    if (existingUser != null)
                    {
                        _logger.LogWarning("Email {Email} ya está registrado por otro usuario", request.Mail);
                        return BadRequest($"El email {request.Mail} ya está registrado");
                    }
                }

                user.Nombre = request.Nombre;
                user.Apellidos = request.Apellidos;
                user.Mail = request.Mail;
                user.Direccion = request.Direccion;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario {UserId} actualizado exitosamente", id);
                return Ok("Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID {UserId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
                    return NotFound($"Usuario con ID {id} no encontrado");
                }

                var hasReservations = await _context.Reservations
                    .AnyAsync(r => r.IdUsuario == id && r.Estado == ReservationStatus.Reservado);
                
                if (hasReservations)
                {
                    _logger.LogWarning("No se puede eliminar usuario {UserId} porque tiene reservas activas", id);
                    return BadRequest("No se puede eliminar el usuario porque tiene reservas activas");
                }

                _context.Users.Remove(user!);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario {UserId} eliminado exitosamente", id);
                return Ok("Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID {UserId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
