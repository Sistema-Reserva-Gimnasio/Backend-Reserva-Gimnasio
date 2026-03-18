using SistemaReservaGimnasioBK.Application.DTOs;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.Services;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Application.CommandHandlers;

/// <summary>
/// Command Handler que procesa el comando CreateBookingCommand.
/// En CQRS, los Handlers contienen la lógica de la capa Application para ejecutar Commands.
/// Coordinan el flujo entre Domain Services, Repositories, y otras dependencias,
/// pero NO contienen lógica de negocio (esa está en la capa Domain).
/// </summary>
public class CreateBookingCommandHandler
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISpaceRepository _spaceRepository;
    private readonly IBookingDomainService _bookingDomainService;

    /// <summary>
    /// Constructor con inyección de dependencias
    /// </summary>
    public CreateBookingCommandHandler(
        IBookingRepository bookingRepository,
        ISpaceRepository spaceRepository,
        IBookingDomainService bookingDomainService)
    {
        _bookingRepository = bookingRepository;
        _spaceRepository = spaceRepository;
        _bookingDomainService = bookingDomainService;
    }

    /// <summary>
    /// Maneja el comando de creación de reserva
    /// Pasos:
    /// 1. Crear el Value Object ReservationPeriod (con validaciones)
    /// 2. Validar disponibilidad usando el Domain Service (invariante de negocio)
    /// 3. Crear la entidad Booking usando el factory method
    /// 4. Persistir la reserva
    /// 5. (Opcionalmente) Publicar Domain Event
    /// </summary>
    public async Task<BookingResponseDto> HandleAsync(Commands.CreateBookingCommand command)
    {
        // Paso 1: Crear el Value Object con validación
        // El Value Object valida que StartDateTime < EndDateTime y que sea futuro
        ReservationPeriod period;
        try
        {
            period = ReservationPeriod.Create(command.StartDateTime, command.EndDateTime);
        }
        catch (ArgumentException ex)
        {
            throw new InvalidOperationException($"Período de reserva inválido: {ex.Message}", ex);
        }

        // Paso 2: Validar la invariante de negocio usando Domain Service
        // Esto verifica que el espacio no esté ocupado en ese horario
        await _bookingDomainService.ValidateBookingCreationAsync(command.SpaceId, period);

        // Paso 3: Crear la entidad Booking
        var booking = Booking.Create(command.SpaceId, command.UserId, period, command.Notes);

        // Confirmar automáticamente la reserva (regla de negocio simple)
        booking.Confirm();

        // Paso 4: Persistir
        var createdBooking = await _bookingRepository.AddAsync(booking);

        // Paso 5: Obtener el nombre del espacio para la respuesta
        var space = await _spaceRepository.GetByIdAsync(command.SpaceId);

        // TODO: Aquí se podría publicar un BookingCreatedEvent
        // para que otros bounded contexts o servicios reaccionen
        // Ejemplo: enviar email de confirmación, actualizar estadísticas, etc.

        // Mapear a DTO de respuesta
        return new BookingResponseDto
        {
            Id = createdBooking.Id,
            SpaceId = createdBooking.SpaceId,
            SpaceName = space?.Name,
            UserId = createdBooking.UserId,
            StartDateTime = createdBooking.Period.StartDateTime,
            EndDateTime = createdBooking.Period.EndDateTime,
            Status = createdBooking.Status.ToString(),
            Notes = createdBooking.Notes,
            CreatedAt = createdBooking.CreatedAt
        };
    }
}
