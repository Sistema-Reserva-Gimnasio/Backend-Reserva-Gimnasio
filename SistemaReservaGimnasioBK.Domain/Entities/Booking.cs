using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Domain.Entities;

/// <summary>
/// Entidad que representa una reserva de un espacio.
/// En DDD, esta es una entidad dentro del Aggregate de Space, pero también puede ser
/// un Aggregate Root si necesitamos accederla directamente (lo cual es común en sistemas de reservas).
/// </summary>
public class Booking
{
    /// <summary>
    /// Identificador único de la reserva (Primary Key)
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// ID del espacio reservado (Foreign Key)
    /// </summary>
    public Guid SpaceId { get; private set; }

    /// <summary>
    /// Navegación al espacio reservado
    /// En DDD, las navegaciones representan relaciones entre Aggregates
    /// </summary>
    public Space? Space { get; private set; }

    /// <summary>
    /// ID del usuario que realizó la reserva
    /// En un sistema completo, esto sería una referencia a un Aggregate de User
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Período de tiempo de la reserva (Value Object)
    /// En DDD, encapsulamos conceptos complejos en Value Objects
    /// </summary>
    public ReservationPeriod Period { get; private set; }

    /// <summary>
    /// Estado actual de la reserva (Value Object)
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Notas adicionales de la reserva
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Fecha de creación de la reserva
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Constructor privado para uso de EF Core
    /// </summary>
    private Booking() { }

    /// <summary>
    /// Constructor privado usado por el factory method
    /// </summary>
    private Booking(Guid id, Guid spaceId, Guid userId, ReservationPeriod period, string? notes)
    {
        Id = id;
        SpaceId = spaceId;
        UserId = userId;
        Period = period;
        Notes = notes;
        Status = BookingStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method para crear una nueva reserva
    /// En DDD, este método garantiza que la reserva se crea en un estado válido
    /// NOTA: La validación de disponibilidad del espacio NO se hace aquí,
    /// sino en el BookingDomainService (invariante de negocio que requiere consultar repositorio)
    /// </summary>
    public static Booking Create(Guid spaceId, Guid userId, ReservationPeriod period, string? notes = null)
    {
        if (spaceId == Guid.Empty)
            throw new ArgumentException("El ID del espacio es requerido", nameof(spaceId));

        if (userId == Guid.Empty)
            throw new ArgumentException("El ID del usuario es requerido", nameof(userId));

        if (period == null)
            throw new ArgumentNullException(nameof(period), "El período de reserva es requerido");

        return new Booking(Guid.NewGuid(), spaceId, userId, period, notes);
    }

    /// <summary>
    /// Confirma la reserva
    /// En DDD, los métodos expresan acciones del dominio, no solo setters
    /// </summary>
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new InvalidOperationException("Solo se pueden confirmar reservas en estado Pendiente");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancela la reserva
    /// </summary>
    public void Cancel()
    {
        if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("No se puede cancelar una reserva completada o ya cancelada");

        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marca la reserva como en progreso (el usuario está usando el espacio)
    /// </summary>
    public void StartUsage()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Solo se pueden iniciar reservas confirmadas");

        // Validación: solo se puede iniciar cerca del horario de inicio
        var now = DateTime.UtcNow;
        if (now < Period.StartDateTime.AddMinutes(-15) || now > Period.StartDateTime.AddHours(1))
            throw new InvalidOperationException("La reserva solo puede iniciarse cerca de su horario programado");

        Status = BookingStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Completa la reserva
    /// </summary>
    public void Complete()
    {
        if (Status != BookingStatus.InProgress)
            throw new InvalidOperationException("Solo se pueden completar reservas en progreso");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza las notas de la reserva
    /// </summary>
    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}
