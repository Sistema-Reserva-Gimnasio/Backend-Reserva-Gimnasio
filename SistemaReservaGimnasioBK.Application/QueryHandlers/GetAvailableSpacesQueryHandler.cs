using SistemaReservaGimnasioBK.Application.DTOs;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Domain.Services;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Application.QueryHandlers;

/// <summary>
/// Query Handler que procesa GetAvailableSpacesQuery.
/// En CQRS, los Query Handlers ejecutan operaciones de lectura y devuelven DTOs.
/// Pueden optimizarse para lectura sin preocuparse por la consistencia total del aggregate.
/// </summary>
public class GetAvailableSpacesQueryHandler
{
    private readonly ISpaceRepository _spaceRepository;
    private readonly IBookingDomainService _bookingDomainService;

    public GetAvailableSpacesQueryHandler(ISpaceRepository spaceRepository, IBookingDomainService bookingDomainService)
    {
        _spaceRepository = spaceRepository;
        _bookingDomainService = bookingDomainService;
    }

    /// <summary>
    /// Obtiene todos los espacios disponibles en el período solicitado
    /// </summary>
    public async Task<IEnumerable<SpaceDto>> HandleAsync(Queries.GetAvailableSpacesQuery query)
    {
        // Crear el período de reserva
        var period = ReservationPeriod.Create(query.StartDateTime, query.EndDateTime);

        // Obtener espacios según el filtro de tipo (si existe)
        var spaces = query.SpaceType.HasValue
            ? await _spaceRepository.GetByTypeAsync(query.SpaceType.Value)
            : await _spaceRepository.GetActiveSpacesAsync();

        // Filtrar solo espacios disponibles
        var availableSpaces = new List<SpaceDto>();

        foreach (var space in spaces)
        {
            var isAvailable = await _bookingDomainService.IsSpaceAvailableAsync(space.Id, period);
            
            if (isAvailable)
            {
                availableSpaces.Add(new SpaceDto
                {
                    Id = space.Id,
                    Name = space.Name,
                    Description = space.Description,
                    Capacity = space.Capacity,
                    Type = space.Type.ToString(),
                    IsActive = space.IsActive
                });
            }
        }

        return availableSpaces;
    }
}
