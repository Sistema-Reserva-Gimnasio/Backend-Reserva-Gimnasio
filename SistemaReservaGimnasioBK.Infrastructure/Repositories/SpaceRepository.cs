using Microsoft.EntityFrameworkCore;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.ValueObjects;
using SistemaReservaGimnasioBK.Infrastructure.Persistence;

namespace SistemaReservaGimnasioBK.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio ISpaceRepository usando Entity Framework Core.
/// En DDD, los repositorios concretos están en la capa Infrastructure
/// y proporcionan la implementación real del acceso a datos.
/// 
/// El patrón Repository abstrae el acceso a datos y hace que el dominio
/// sea independiente de la tecnología de persistencia utilizada.
/// </summary>
public class SpaceRepository : ISpaceRepository
{
    private readonly ApplicationDbContext _context;

    public SpaceRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Space?> GetByIdAsync(Guid id)
    {
        return await _context.Spaces
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Space>> GetAllAsync()
    {
        return await _context.Spaces
            .ToListAsync();
    }

    public async Task<IEnumerable<Space>> GetActiveSpacesAsync()
    {
        return await _context.Spaces
            .Where(s => s.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Space>> GetByTypeAsync(SpaceType type)
    {
        return await _context.Spaces
            .Where(s => s.Type == type && s.IsActive)
            .ToListAsync();
    }

    public async Task<Space> AddAsync(Space space)
    {
        await _context.Spaces.AddAsync(space);
        await _context.SaveChangesAsync();
        return space;
    }

    public async Task UpdateAsync(Space space)
    {
        _context.Spaces.Update(space);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var space = await GetByIdAsync(id);
        if (space != null)
        {
            _context.Spaces.Remove(space);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        return await _context.Spaces
            .AnyAsync(s => s.Name == name && (excludeId == null || s.Id != excludeId.Value));
    }
}
