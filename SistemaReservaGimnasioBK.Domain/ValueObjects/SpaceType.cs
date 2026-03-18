namespace SistemaReservaGimnasioBK.Domain.ValueObjects;

/// <summary>
/// Value Object que representa el tipo de espacio reservable.
/// En DDD, los enums se tratan como Value Objects simples que representan
/// conceptos del dominio (vocabulario ubicuo).
/// Aplicable tanto para coworking como gimnasios.
/// </summary>
public enum SpaceType
{
    /// <summary>
    /// Escritorio individual en coworking
    /// </summary>
    Desk = 1,

    /// <summary>
    /// Sala de reuniones en coworking
    /// </summary>
    MeetingRoom = 2,

    /// <summary>
    /// Área de gimnasio (zona de cardio, pesas, etc.)
    /// </summary>
    GymArea = 3,

    /// <summary>
    /// Zona de entrenamiento específica (sala de spinning, yoga, etc.)
    /// </summary>
    TrainingZone = 4,

    /// <summary>
    /// Oficina privada en coworking
    /// </summary>
    PrivateOffice = 5,

    /// <summary>
    /// Casillero o locker en gimnasio
    /// </summary>
    Locker = 6
}
