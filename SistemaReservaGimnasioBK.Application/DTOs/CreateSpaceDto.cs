namespace SistemaReservaGimnasioBK.Application.DTOs;

/// <summary>
/// DTO para crear un nuevo espacio
/// </summary>
public class CreateSpaceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Type { get; set; } // SpaceType enum como int
}
