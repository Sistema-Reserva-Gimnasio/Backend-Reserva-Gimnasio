using Microsoft.EntityFrameworkCore;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Infrastructure.Persistence.Configurations;

namespace SistemaReservaGimnasioBK.Infrastructure.Persistence;

/// <summary>
/// DbContext de Entity Framework Core para el sistema de reservas.
/// En DDD, el DbContext está en la capa Infrastructure y configura cómo
/// las entidades del dominio se persisten en la base de datos.
/// 
/// IMPORTANTE: Los Value Objects se persisten como parte de las entidades
/// que los contienen, no como tablas separadas (configuración OwnsOne).
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// DbSet para la entidad Space (Aggregate Root)
    /// </summary>
    public DbSet<Space> Spaces { get; set; }

    /// <summary>
    /// DbSet para la entidad Booking (Aggregate Root)
    /// </summary>
    public DbSet<Booking> Bookings { get; set; }

    /// <summary>
    /// Constructor para inyección de dependencias
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configuración del modelo usando Fluent API
    /// En DDD, usamos Fluent API en lugar de Data Annotations para mantener
    /// las entidades del dominio libres de atributos relacionados con persistencia
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones desde clases separadas (mejor organización)
        modelBuilder.ApplyConfiguration(new SpaceConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());

        // Alternativa: aplicar todas las configuraciones del assembly
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Intercepta el guardado para agregar funcionalidad adicional si es necesario
    /// Por ejemplo: publicar Domain Events, auditar cambios, etc.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Aquí se podría implementar lógica adicional:
        // - Publicación de Domain Events
        // - Auditoría automática (CreatedAt, UpdatedAt)
        // - Validaciones finales
        
        return await base.SaveChangesAsync(cancellationToken);
    }
}
