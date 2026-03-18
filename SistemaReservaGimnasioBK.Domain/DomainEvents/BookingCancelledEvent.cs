namespace SistemaReservaGimnasioBK.Domain.DomainEvents;

/// <summary>
/// Domain Event que se dispara cuando una reserva es cancelada.
/// Puede ser usado para notificar al usuario, liberar el espacio, etc.
/// </summary>
public sealed class BookingCancelledEvent
{
    /// <summary>
    /// ID de la reserva cancelada
    /// </summary>
    public Guid BookingId { get; }

    /// <summary>
    /// ID del espacio que se liberó
    /// </summary>
    public Guid SpaceId { get; }

    /// <summary>
    /// ID del usuario cuya reserva fue cancelada
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Motivo de la cancelación (opcional)
    /// </summary>
    public string? CancellationReason { get; }

    /// <summary>
    /// Fecha y hora en que ocurrió el evento
    /// </summary>
    public DateTime OccurredOn { get; }

    public BookingCancelledEvent(Guid bookingId, Guid spaceId, Guid userId, string? cancellationReason = null)
    {
        BookingId = bookingId;
        SpaceId = spaceId;
        UserId = userId;
        CancellationReason = cancellationReason;
        OccurredOn = DateTime.UtcNow;
    }
}
