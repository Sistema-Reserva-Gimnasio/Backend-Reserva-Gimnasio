using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using SistemaReservaGimnasioBK.Application.Commands;
using SistemaReservaGimnasioBK.Application.CommandHandlers;
using SistemaReservaGimnasioBK.Application.DTOs;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Controllers;

/// <summary>
/// Controlador REST para gestión de reservas.
/// En arquitectura DDD, los controladores (API Layer) son responsables de:
/// 1. Recibir requests HTTP y validar DTOs
/// 2. Delegar la ejecución a Command/Query Handlers (Application Layer)
/// 3. Convertir resultados a respuestas HTTP apropiadas
/// 
/// Los controladores NO deben contener lógica de negocio.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly CreateBookingCommandHandler _createBookingHandler;
    private readonly IBookingRepository _bookingRepository;
    private readonly IValidator<CreateBookingCommand> _validator;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        CreateBookingCommandHandler createBookingHandler,
        IBookingRepository bookingRepository,
        IValidator<CreateBookingCommand> validator,
        ILogger<BookingsController> logger)
    {
        _createBookingHandler = createBookingHandler;
        _bookingRepository = bookingRepository;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva reserva
    /// </summary>
    /// <param name="dto">Datos de la reserva</param>
    /// <returns>Reserva creada</returns>
    /// <response code="201">Reserva creada exitosamente</response>
    /// <response code="400">Datos inválidos o espacio no disponible</response>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        try
        {
            // Crear el comando desde el DTO
            var command = new CreateBookingCommand(
                dto.SpaceId,
                dto.UserId,
                dto.StartDateTime,
                dto.EndDateTime,
                dto.Notes);

            // Validar con FluentValidation
            var validationResult = await _validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
                });
            }

            // Ejecutar el Command Handler (Application Layer)
            var result = await _createBookingHandler.HandleAsync(command);

            _logger.LogInformation("Reserva creada: {BookingId} para espacio {SpaceId}", result.Id, result.SpaceId);

            return CreatedAtAction(nameof(GetBooking), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            // Errores de negocio (ej: espacio no disponible)
            _logger.LogWarning(ex, "Error de negocio al crear reserva");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear reserva");
            return StatusCode(500, new { message = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene una reserva por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
            return NotFound(new { message = $"Reserva {id} no encontrada" });

        var response = new BookingResponseDto
        {
            Id = booking.Id,
            SpaceId = booking.SpaceId,
            SpaceName = booking.Space?.Name,
            UserId = booking.UserId,
            StartDateTime = booking.Period.StartDateTime,
            EndDateTime = booking.Period.EndDateTime,
            Status = booking.Status.ToString(),
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// Obtiene todas las reservas de un usuario
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserBookings(Guid userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);

        var response = bookings.Select(b => new BookingResponseDto
        {
            Id = b.Id,
            SpaceId = b.SpaceId,
            SpaceName = b.Space?.Name,
            UserId = b.UserId,
            StartDateTime = b.Period.StartDateTime,
            EndDateTime = b.Period.EndDateTime,
            Status = b.Status.ToString(),
            Notes = b.Notes,
            CreatedAt = b.CreatedAt
        });

        return Ok(response);
    }

    /// <summary>
    /// Cancela una reserva
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
            return NotFound(new { message = $"Reserva {id} no encontrada" });

        try
        {
            booking.Cancel();
            await _bookingRepository.UpdateAsync(booking);

            _logger.LogInformation("Reserva cancelada: {BookingId}", id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
