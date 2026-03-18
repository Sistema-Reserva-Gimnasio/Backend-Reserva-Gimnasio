using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Application.DTOs;

/// <summary>
/// DTO de respuesta con información de una reserva.
/// Se usa para devolver datos al cliente después de crear o consultar una reserva.
/// </summary>
public class BookingResponseDto
{
    public Guid Id { get; set; }
    public Guid SpaceId { get; set; }
    public string? SpaceName { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
