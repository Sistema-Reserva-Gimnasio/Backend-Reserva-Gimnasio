using SistemaReservaGimnasioBK.Domain.Entities;

namespace SistemaReservaGimnasioBK.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio para la entidad Space.
/// En DDD, las interfaces de repositorio se definen en la capa Domain
/// pero se implementan en la capa Infrastructure.
/// Esto permite que el dominio sea independiente de la tecnología de persistencia.
/// 
/// El patrón Repository abstrae el acceso a datos y proporciona
/// una interfaz orientada a colecciones para trabajar con Aggregates.
/// </summary>
public interface ISpaceRepository
{
    /// <summary>
    /// Obtiene un espacio por su ID
    /// </summary>
    Task<Space?> GetByIdAsync(Guid id);

    /// <summary>
    /// Obtiene todos los espacios
    /// </summary>
    Task<IEnumerable<Space>> GetAllAsync();

    /// <summary>
    /// Obtiene todos los espacios activos
    /// </summary>
    Task<IEnumerable<Space>> GetActiveSpacesAsync();

    /// <summary>
    /// Obtiene espacios filtrados por tipo
    /// </summary>
    Task<IEnumerable<Space>> GetByTypeAsync(ValueObjects.SpaceType type);

    /// <summary>
    /// Agrega un nuevo espacio
    /// En DDD, los repositorios trabajan con Aggregates completos
    /// </summary>
    Task<Space> AddAsync(Space space);

    /// <summary>
    /// Actualiza un espacio existente
    /// </summary>
    Task UpdateAsync(Space space);

    /// <summary>
    /// Elimina un espacio
    /// Nota: En producción, se recomienda soft delete (marcar como inactivo)
    /// en lugar de eliminación física
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Verifica si existe un espacio con el nombre especificado
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
}
