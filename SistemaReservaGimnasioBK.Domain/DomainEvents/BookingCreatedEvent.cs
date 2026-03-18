using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Domain.DomainEvents;

/// <summary>
/// Domain Event que se dispara cuando se crea una nueva reserva.
/// En DDD, los Domain Events representan hechos importantes que ocurrieron en el dominio
/// y que otras partes del sistema podrían necesitar conocer.
/// Los eventos son inmutables y representan algo que YA OCURRIÓ (pasado).
/// </summary>
public sealed class BookingCreatedEvent
{
    /// <summary>
    /// ID de la reserva creada
    /// </summary>
    public Guid BookingId { get; }

    /// <summary>
    /// ID del espacio reservado
    /// </summary>
    public Guid SpaceId { get; }

    /// <summary>
    /// ID del usuario que realizó la reserva
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Período de tiempo de la reserva
    /// </summary>
    public ReservationPeriod Period { get; }

    /// <summary>
    /// Fecha y hora en que ocurrió el evento
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Constructor del evento
    /// Los eventos son inmutables - todos los valores se establecen en la construcción
    /// </summary>
    public BookingCreatedEvent(Guid bookingId, Guid spaceId, Guid userId, ReservationPeriod period)
    {
        BookingId = bookingId;
        SpaceId = spaceId;
        UserId = userId;
        Period = period;
        OccurredOn = DateTime.UtcNow;
    }
}
