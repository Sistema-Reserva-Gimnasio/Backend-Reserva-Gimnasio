namespace SistemaReservaGimnasioBK.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un período de reserva con fecha/hora de inicio y fin.
/// En DDD, los Value Objects son inmutables y se comparan por valor, no por identidad.
/// Este VO encapsula la lógica de validación de períodos y detección de solapamientos.
/// </summary>
public sealed class ReservationPeriod : IEquatable<ReservationPeriod>
{
    /// <summary>
    /// Fecha y hora de inicio de la reserva
    /// </summary>
    public DateTime StartDateTime { get; }

    /// <summary>
    /// Fecha y hora de fin de la reserva
    /// </summary>
    public DateTime EndDateTime { get; }

    /// <summary>
    /// Constructor privado para garantizar la creación solo mediante el método factory
    /// </summary>
    private ReservationPeriod(DateTime startDateTime, DateTime endDateTime)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
    }

    /// <summary>
    /// Factory method para crear una instancia de ReservationPeriod con validación
    /// </summary>
    /// <param name="startDateTime">Fecha y hora de inicio</param>
    /// <param name="endDateTime">Fecha y hora de fin</param>
    /// <returns>Instancia válida de ReservationPeriod</returns>
    /// <exception cref="ArgumentException">Si las fechas no son válidas</exception>
    public static ReservationPeriod Create(DateTime startDateTime, DateTime endDateTime)
    {
        // Validación: la fecha de inicio debe ser anterior a la de fin
        if (startDateTime >= endDateTime)
        {
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");
        }

        // Validación: la fecha de inicio debe ser en el futuro (regla de negocio)
        if (startDateTime < DateTime.UtcNow)
        {
            throw new ArgumentException("La fecha de inicio debe ser en el futuro");
        }

        return new ReservationPeriod(startDateTime, endDateTime);
    }

    /// <summary>
    /// Verifica si este período se solapa con otro período
    /// Esta es la lógica clave para validar la invariante de negocio:
    /// "No se puede reservar un espacio si ya está ocupado en ese horario"
    /// </summary>
    /// <param name="other">Otro período de reserva</param>
    /// <returns>True si hay solapamiento, False en caso contrario</returns>
    public bool Overlaps(ReservationPeriod other)
    {
        if (other == null)
            return false;

        // Dos períodos se solapan si:
        // - El inicio de uno está dentro del otro, O
        // - El fin de uno está dentro del otro, O
        // - Uno contiene completamente al otro
        return StartDateTime < other.EndDateTime && EndDateTime > other.StartDateTime;
    }

    /// <summary>
    /// Calcula la duración del período en horas
    /// </summary>
    public double DurationInHours => (EndDateTime - StartDateTime).TotalHours;

    #region Implementación de igualdad por valor (característica clave de Value Objects)

    public bool Equals(ReservationPeriod? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartDateTime.Equals(other.StartDateTime) && EndDateTime.Equals(other.EndDateTime);
    }

    public override bool Equals(object? obj)
    {
        return obj is ReservationPeriod other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDateTime, EndDateTime);
    }

    public static bool operator ==(ReservationPeriod? left, ReservationPeriod? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ReservationPeriod? left, ReservationPeriod? right)
    {
        return !Equals(left, right);
    }

    #endregion

    public override string ToString()
    {
        return $"{StartDateTime:g} - {EndDateTime:g} ({DurationInHours:F1}h)";
    }
}
