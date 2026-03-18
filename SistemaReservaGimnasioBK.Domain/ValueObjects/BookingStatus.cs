namespace SistemaReservaGimnasioBK.Domain.ValueObjects;

/// <summary>
/// Value Object que representa el estado de una reserva.
/// En DDD, los estados de las entidades se modelan como Value Objects
/// para garantizar transiciones válidas y aplicar reglas de negocio.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Reserva creada pero pendiente de confirmación
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Reserva confirmada y activa
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Reserva cancelada por el usuario o el sistema
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Reserva completada (el período ya finalizó)
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Reserva en uso (el usuario está actualmente en el espacio)
    /// </summary>
    InProgress = 5
}
