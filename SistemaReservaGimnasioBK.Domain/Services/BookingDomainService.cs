using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Domain.Services;

/// <summary>
/// Implementación del Domain Service para validación de reservas.
/// NOTA: Aunque esta es una implementación concreta, está en la capa Domain
/// porque contiene lógica de negocio pura. La dependencia del repositorio
/// se inyecta a través de la interfaz (que también está en Domain).
/// </summary>
public class BookingDomainService : IBookingDomainService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ISpaceRepository _spaceRepository;

    /// <summary>
    /// Constructor con inyección de dependencias
    /// Los repositorios se definen como interfaces en Domain
    /// pero se implementan en Infrastructure
    /// </summary>
    public BookingDomainService(IBookingRepository bookingRepository, ISpaceRepository spaceRepository)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _spaceRepository = spaceRepository ?? throw new ArgumentNullException(nameof(spaceRepository));
    }

    /// <summary>
    /// Verifica si un espacio está disponible consultando reservas existentes
    /// Esta es la implementación de la invariante de negocio principal
    /// </summary>
    public async Task<bool> IsSpaceAvailableAsync(Guid spaceId, ReservationPeriod period, Guid? excludeBookingId = null)
    {
        // Primero verificamos que el espacio exista y esté activo
        var space = await _spaceRepository.GetByIdAsync(spaceId);
        if (space == null || !space.IsActive)
            return false;

        // Obtener todas las reservas activas para este espacio en el período
        var existingBookings = await _bookingRepository.GetBookingsBySpaceAndPeriodAsync(spaceId, period);

        // Filtrar solo las reservas que están confirmadas o en progreso (no canceladas)
        var activeBookings = existingBookings
            .Where(b => b.Status == BookingStatus.Confirmed || 
                       b.Status == BookingStatus.InProgress ||
                       b.Status == BookingStatus.Pending)
            .Where(b => excludeBookingId == null || b.Id != excludeBookingId.Value)
            .ToList();

        // Verificar si alguna reserva activa se solapa con el período solicitado
        // Usamos el método Overlaps del Value Object ReservationPeriod
        foreach (var booking in activeBookings)
        {
            if (booking.Period.Overlaps(period))
            {
                return false; // Hay solapamiento - NO disponible
            }
        }

        return true; // No hay solapamientos - SÍ disponible
    }

    /// <summary>
    /// Valida que una reserva pueda ser creada
    /// Lanza una excepción de dominio si no se puede crear
    /// </summary>
    public async Task ValidateBookingCreationAsync(Guid spaceId, ReservationPeriod period)
    {
        var isAvailable = await IsSpaceAvailableAsync(spaceId, period);

        if (!isAvailable)
        {
            throw new InvalidOperationException(
                $"El espacio no está disponible en el período solicitado " +
                $"({period.StartDateTime:g} - {period.EndDateTime:g})");
        }
    }
}
