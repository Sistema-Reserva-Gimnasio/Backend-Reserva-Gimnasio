using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio para la entidad Booking.
/// Además de las operaciones CRUD estándar, incluye queries especializadas
/// necesarias para la lógica de negocio (validación de disponibilidad).
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Obtiene una reserva por su ID
    /// </summary>
    Task<Booking?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtiene todas las reservas
    /// </summary>
    Task<IEnumerable<Booking>> GetAllAsync();

    /// <summary>
    /// Obtiene reservas por espacio
    /// </summary>
    Task<IEnumerable<Booking>> GetBySpaceIdAsync(Guid spaceId);

    /// <summary>
    /// Obtiene reservas por usuario
    /// </summary>
    Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);

    /// <summary>
    /// Obtiene reservas por estado
    /// </summary>
    Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status);

    /// <summary>
    /// Método clave para validación de disponibilidad:
    /// Obtiene todas las reservas de un espacio que se solapan con un período dado.
    /// Este método es usado por el BookingDomainService para implementar
    /// la invariante de negocio "No se puede reservar un espacio ocupado"
    /// </summary>
    Task<IEnumerable<Booking>> GetBookingsBySpaceAndPeriodAsync(Guid spaceId, ReservationPeriod period);

    /// <summary>
    /// Obtiene reservas activas (no canceladas ni completadas) de un usuario
    /// en un rango de fechas
    /// </summary>
    Task<IEnumerable<Booking>> GetActiveBookingsByUserAsync(Guid userId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Agrega una nueva reserva
    /// </summary>
    Task<Booking> AddAsync(Booking booking);

    /// <summary>
    /// Actualiza una reserva existente
    /// </summary>
    Task UpdateAsync(Booking booking);

    /// <summary>
    /// Elimina una reserva
    /// Nota: Normalmente no se eliminan reservas, solo se cancelan
    /// </summary>
    Task DeleteAsync(Guid id);
}
