using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Space usando Fluent API.
/// En DDD, separamos la configuración de persistencia de las entidades del dominio
/// para mantener la capa Domain independiente de la tecnología de persistencia.
/// </summary>
public class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        // Configuración de la tabla
        builder.ToTable("Spaces");

        // Primary Key
        builder.HasKey(s => s.Id);

        // Propiedades
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.Capacity)
            .IsRequired();

        // Enum como int en la base de datos
        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<int>(); // Convierte el enum SpaceType a int

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);

        // Índices para mejorar rendimiento de consultas
        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.Name).IsUnique(); // Nombre único

        // Configuración de la relación con Bookings
        // Un espacio puede tener muchas reservas
        builder.HasMany(s => s.Bookings)
            .WithOne(b => b.Space)
            .HasForeignKey(b => b.SpaceId)
            .OnDelete(DeleteBehavior.Restrict); // No permitir eliminar un espacio con reservas
    }
}
