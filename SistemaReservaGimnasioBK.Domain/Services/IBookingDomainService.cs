using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Domain.Services;

/// <summary>
/// Domain Service que valida la disponibilidad de espacios.
/// En DDD, los Domain Services se usan cuando:
/// 1. La lógica no pertenece naturalmente a una sola entidad
/// 2. La operación requiere múltiples aggregates o consultas a repositorio
/// 3. La lógica es un concepto del dominio pero no tiene estado propio
/// 
/// Este servicio implementala invariante de negocio clave:
/// "No se puede reservar un espacio si ya está ocupado en ese horario"
/// </summary>
public interface IBookingDomainService
{
    /// <summary>
    /// Verifica si un espacio está disponible en un período específico
    /// </summary>
    /// <param name="spaceId">ID del espacio a verificar</param>
    /// <param name="period">Período de tiempo a verificar</param>
    /// <param name="excludeBookingId">ID de reserva a excluir (útil para ediciones)</param>
    /// <returns>True si el espacio está disponible, False si está ocupado</returns>
    Task<bool> IsSpaceAvailableAsync(Guid spaceId, ReservationPeriod period, Guid? excludeBookingId = null);

    /// <summary>
    /// Valida si una reserva puede ser creada
    /// Lanza excepción si no se puede crear
    /// </summary>
    Task ValidateBookingCreationAsync(Guid spaceId, ReservationPeriod period);
}
