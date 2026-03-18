using Microsoft.EntityFrameworkCore;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.ValueObjects;
using SistemaReservaGimnasioBK.Infrastructure.Persistence;

namespace SistemaReservaGimnasioBK.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio IBookingRepository usando Entity Framework Core.
/// Incluye queries optimizadas para la validación de disponibilidad de espacios.
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.Space) // Eager loading de la navegación
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.Space)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBySpaceIdAsync(Guid spaceId)
    {
        return await _context.Bookings
            .Where(b => b.SpaceId == spaceId)
            .Include(b => b.Space)
            .OrderBy(b => b.Period.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Space)
            .OrderByDescending(b => b.Period.StartDateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status)
    {
        return await _context.Bookings
            .Where(b => b.Status == status)
            .Include(b => b.Space)
            .ToListAsync();
    }

    /// <summary>
    /// MÉTODO CLAVE para validación de disponibilidad.
    /// Obtiene todas las reservas de un espacio que se solapan con un período dado.
    /// Este query es usado por el BookingDomainService para implementar
    /// la invariante de negocio "No se puede reservar un espacio ocupado".
    /// 
    /// ASPECTO IMPORTANTE: Acceso a propiedades del Value Object en LINQ.
    /// EF Core puede traducir Period.StartDateTime y Period.EndDateTime
    /// porque los configuramos con OwnsOne en BookingConfiguration.
    /// </summary>
    public async Task<IEnumerable<Booking>> GetBookingsBySpaceAndPeriodAsync(
        Guid spaceId, 
        ReservationPeriod period)
    {
        return await _context.Bookings
            .Where(b => b.SpaceId == spaceId)
            // Filtrar reservas que se solapan con el período solicitado
            // Una reserva se solapa si su inicio es antes del fin del período solicitado
            // Y su fin es después del inicio del período solicitado
            .Where(b => b.Period.StartDateTime < period.EndDateTime && 
                       b.Period.EndDateTime > period.StartDateTime)
            .Include(b => b.Space)
            .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetActiveBookingsByUserAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate)
    {
        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .Where(b => b.Status == BookingStatus.Confirmed || 
                       b.Status == BookingStatus.InProgress)
            .Where(b => b.Period.StartDateTime >= startDate && 
                       b.Period.EndDateTime <= endDate)
            .Include(b => b.Space)
            .OrderBy(b => b.Period.StartDateTime)
            .ToListAsync();
    }

    public async Task<Booking> AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var booking = await GetByIdAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
