using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Domain.Entities;

/// <summary>
/// Entidad que representa un espacio reservable (escritorio, sala, área de gimnasio).
/// En DDD, esta es un Aggregate Root - controla el acceso a las entidades dentro de su límite.
/// Los Aggregate Roots son las únicas entidades que pueden ser obtenidas directamente desde repositorios.
/// </summary>
public class Space
{
    /// <summary>
    /// Identificador único del espacio (Primary Key)
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Nombre descriptivo del espacio
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Descripción detallada del espacio y sus características
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Capacidad del espacio (número de personas que pueden usarlo simultáneamente)
    /// </summary>
    public int Capacity { get; private set; }

    /// <summary>
    /// Tipo de espacio (Value Object)
    /// </summary>
    public SpaceType Type { get; private set; }

    /// <summary>
    /// Indica si el espacio está activo y disponible para reservas
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Fecha de creación del espacio
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Colección de reservas asociadas a este espacio
    /// En DDD, las relaciones se gestionan a través del Aggregate Root
    /// </summary>
    private readonly List<Booking> _bookings = new();
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    /// <summary>
    /// Constructor privado para uso de EF Core
    /// En DDD, las entidades no deben tener constructores públicos sin parámetros
    /// </summary>
    private Space() { }

    /// <summary>
    /// Constructor privado usado por el factory method
    /// </summary>
    private Space(Guid id, string name, string description, int capacity, SpaceType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Capacity = capacity;
        Type = type;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Factory method para crear un nuevo espacio con validación
    /// En DDD, preferimos factory methods sobre constructores públicos
    /// para garantizar que las entidades siempre se crean en un estado válido
    /// </summary>
    public static Space Create(string name, string description, int capacity, SpaceType type)
    {
        // Validaciones de negocio
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del espacio es requerido", nameof(name));

        if (capacity <= 0)
            throw new ArgumentException("La capacidad debe ser mayor a cero", nameof(capacity));

        return new Space(Guid.NewGuid(), name, description, capacity, type);
    }

    /// <summary>
    /// Actualiza la información del espacio
    /// En DDD, los métodos de negocio deben expresar la intención del usuario
    /// </summary>
    public void Update(string name, string description, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del espacio es requerido", nameof(name));

        if (capacity <= 0)
            throw new ArgumentException("La capacidad debe ser mayor a cero", nameof(capacity));

        Name = name;
        Description = description;
        Capacity = capacity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Desactiva el espacio para que no pueda ser reservado
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Reactiva el espacio
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
