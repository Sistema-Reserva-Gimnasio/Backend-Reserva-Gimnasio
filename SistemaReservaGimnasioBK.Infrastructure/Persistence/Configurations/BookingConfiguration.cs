using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaReservaGimnasioBK.Domain.Entities;
using SistemaReservaGimnasioBK.Domain.ValueObjects;

namespace SistemaReservaGimnasioBK.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Booking.
/// ASPECTO CLAVE DE DDD: Configuración de Value Objects con OwnsOne.
/// El Value Object ReservationPeriod se persiste como parte de la tabla Bookings,
/// no como una tabla separada, ya que los Value Objects no tienen identidad propia.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // Configuración de la tabla
        builder.ToTable("Bookings");

        // Primary Key
        builder.HasKey(b => b.Id);

        // Propiedades escalares
        builder.Property(b => b.SpaceId)
            .IsRequired();

        builder.Property(b => b.UserId)
            .IsRequired();

        // Enum BookingStatus como int
        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.Notes)
            .HasMaxLength(500);

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired(false);

        // CONFIGURACIÓN CLAVE EN DDD: Persistencia de Value Object
        // OwnsOne indica que ReservationPeriod es un Value Object que pertenece a Booking
        // Los campos del VO se mapean a columnas en la misma tabla (Bookings)
        builder.OwnsOne(b => b.Period, period =>
        {
            // Mapear las propiedades del Value Object a columnas específicas
            period.Property(p => p.StartDateTime)
                .HasColumnName("StartDateTime")
                .IsRequired();

            period.Property(p => p.EndDateTime)
                .HasColumnName("EndDateTime")
                .IsRequired();

            // NOTA: No configuramos PK para el Value Object porque
            // no tiene identidad propia - es parte de Booking
        });

        // Índices para mejorar el rendimiento de consultas
        // Estos índices son cruciales para la query de validación de disponibilidad
        builder.HasIndex(b => b.SpaceId);
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.Status);
        
        // Índice compuesto para optimizar la búsqueda de disponibilidad
        // (usado en GetBookingsBySpaceAndPeriodAsync del repositorio)
        builder.HasIndex(b => new { b.SpaceId, b.Status })
            .HasDatabaseName("IX_Bookings_Space_Status");

        // Relación con Space ya está configurada en SpaceConfiguration
        // pero podemos agregar navegación si es necesario
        builder.HasOne(b => b.Space)
            .WithMany(s => s.Bookings)
            .HasForeignKey(b => b.SpaceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
