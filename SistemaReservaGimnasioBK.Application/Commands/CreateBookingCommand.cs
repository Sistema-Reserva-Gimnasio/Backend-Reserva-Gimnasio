namespace SistemaReservaGimnasioBK.Application.Commands;

/// <summary>
/// Command para crear una nueva reserva.
/// En CQRS (Command Query Responsibility Segregation), los Commands representan
/// intenciones de cambiar el estado del sistema (operaciones de escritura).
/// Los Commands son objetos que encapsulan todos los datos necesarios para ejecutar una acción.
/// </summary>
public class CreateBookingCommand
{
    public Guid SpaceId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Notes { get; set; }

    public CreateBookingCommand(Guid spaceId, Guid userId, DateTime startDateTime, DateTime endDateTime, string? notes = null)
    {
        SpaceId = spaceId;
        UserId = userId;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Notes = notes;
    }
}
