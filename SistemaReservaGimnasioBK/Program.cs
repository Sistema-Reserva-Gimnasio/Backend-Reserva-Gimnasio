using Microsoft.EntityFrameworkCore;
using FluentValidation;
using SistemaReservaGimnasioBK.Infrastructure.Persistence;
using SistemaReservaGimnasioBK.Domain.Repositories;
using SistemaReservaGimnasioBK.Infrastructure.Repositories;
using SistemaReservaGimnasioBK.Domain.Services;
using SistemaReservaGimnasioBK.Application.CommandHandlers;
using SistemaReservaGimnasioBK.Application.QueryHandlers;
using SistemaReservaGimnasioBK.Application.Validators;

/// <summary>
/// Configuración principal de la aplicación WebAPI.
/// En arquitectura DDD, Program.cs (capa Presentation/WebAPI) es responsable de:
/// 1. Configurar Dependency Injection para todas las capas
/// 2. Configurar middlewares y pipeline HTTP
/// 3. Conectar todas las piezas de la arquitectura
/// </summary>

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURACIÓN DE SERVICIOS (Dependency Injection Container)
// ============================================================================

// 1. Configurar DbContext con SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=SistemaReservasDB;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 2. Registrar Repositorios (Infrastructure -> Domain interfaces)
// El patrón Repository permite que el dominio sea independiente de la persistencia
builder.Services.AddScoped<ISpaceRepository, SpaceRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// 3. Registrar Domain Services
// Los Domain Services contienen lógica de negocio que no pertenece a una sola entidad
builder.Services.AddScoped<IBookingDomainService, BookingDomainService>();

// 4. Registrar Command Handlers (Application Layer - Write operations)
builder.Services.AddScoped<CreateBookingCommandHandler>();

// 5. Registrar Query Handlers (Application Layer - Read operations)
builder.Services.AddScoped<GetAvailableSpacesQueryHandler>();

// 6. Configurar FluentValidation
// Registra automáticamente todos los validadores del assembly de Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateBookingCommandValidator>();

// 7. Configurar Controladores
builder.Services.AddControllers();

// 8. Configurar Swagger/OpenAPI para documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sistema de Reservas API",
        Version = "v1",
        Description = "API REST para sistema de reservas de espacios (coworking/gimnasios) usando arquitectura DDD",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Sistema de Reservas",
            Email = "info@sistemareservas.com"
        }
    });
});

// 9. Configurar CORS si es necesario
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================================================================
// CONFIGURACIÓN DEL PIPELINE HTTP (Middlewares)
// ============================================================================

var app = builder.Build();

// Aplicar migraciones automáticamente en desarrollo (opcional)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // db.Database.Migrate(); // Descomentar para aplicar migraciones automáticamente
    }
}

// Configurar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Reservas API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz (http://localhost:5000/)
    });
}

// Middleware de redirección HTTPS
app.UseHttpsRedirection();

// Middleware de CORS
app.UseCors("AllowAll");

// Middleware de autorización (para futuras implementaciones de autenticación)
app.UseAuthorization();

// Mapear controladores
app.MapControllers();

// Endpoint de salud simple
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();