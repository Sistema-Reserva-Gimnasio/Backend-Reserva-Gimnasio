using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Application.Queries;

/// <summary>
/// Query para obtener espacios disponibles en un período.
/// En CQRS, las Queries representan intenciones de obtener datos (operaciones de lectura).
/// No modifican el estado del sistema.
/// </summary>
public class GetAvailableSpacesQuery
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public SpaceType? SpaceType { get; set; }

    public GetAvailableSpacesQuery(DateTime startDateTime, DateTime endDateTime, SpaceType? spaceType = null)
    {
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        SpaceType = spaceType;
    }
}
