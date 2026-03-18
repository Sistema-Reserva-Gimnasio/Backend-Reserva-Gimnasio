namespace SistemaReservaGimnasioBK.Application.DTOs;

/// <summary>
/// DTO para crear una nueva reserva.
/// En DDD, los DTOs (Data Transfer Objects) son objetos simples usados para
/// transferir datos entre las capas de Application y Presentation (WebAPI).
/// No contienen lógica de negocio, solo datos.
/// </summary>
public class CreateBookingDto
{
    /// <summary>
    /// ID del espacio a reservar
    /// </summary>
    public Guid SpaceId { get; set; }

    /// <summary>
    /// ID del usuario que realiza la reserva
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Fecha y hora de inicio de la reserva
    /// </summary>
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// Fecha y hora de fin de la reserva
    /// </summary>
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Notas adicionales (opcional)
    /// </summary>
    public string? Notes { get; set; }
}
