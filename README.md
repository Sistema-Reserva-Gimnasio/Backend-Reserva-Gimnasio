# Sistema de Reservas - Backend .NET 8 con DDD

Backend para sistema de reservas de espacios (coworking/gimnasios) implementado con **.NET 8** y arquitectura **Domain-Driven Design (DDD)**.

## 🏗️ Arquitectura

El proyecto está estructurado en **4 capas** siguiendo principios DDD:

```
├── SistemaReservaGimnasioBK.Domain/         # Lógica de negocio pura
├── SistemaReservaGimnasioBK.Application/    # Casos de uso (CQRS)
├── SistemaReservaGimnasioBK.Infrastructure/ # EF Core, repositorios
└── SistemaReservaGimnasioBK/                # API REST (controladores)
```

## 🚀 Características Implementadas

### Domain Layer (DDD)
- ✅ **Value Objects**: `ReservationPeriod` con lógica de solapamiento
- ✅ **Entities**: `Space`, `Booking` con factory methods
- ✅ **Domain Service**: Validación de disponibilidad (invariante de negocio)
- ✅ **Domain Events**: `BookingCreated`, `BookingCancelled`
- ✅ **Repository Pattern**: Interfaces en Domain

### Application Layer (CQRS)
- ✅ **Commands**: `CreateBookingCommand` + Handler
- ✅ **Queries**: `GetAvailableSpacesQuery` + Handler
- ✅ **FluentValidation**: Validadores para DTOs y Commands
- ✅ **DTOs**: Separación entre dominio y presentación

### Infrastructure Layer
- ✅ **Entity Framework Core 8.0**
- ✅ **Fluent API Configuration**: Mapeo sin contaminar entidades
- ✅ **Value Object Persistence**: Configuración `OwnsOne` para `ReservationPeriod`
- ✅ **Repository Implementations**: Consultas optimizadas con LINQ

### WebAPI Layer
- ✅ **REST Controllers**: `BookingsController`, `SpacesController`
- ✅ **Swagger/OpenAPI**: Documentación automática
- ✅ **Dependency Injection**: Configuración completa en `Program.cs`
- ✅ **CORS**: Configurado para desarrollo

## ⚙️ Requisitos Previos

- .NET 8 SDK
- SQL Server / SQL Server LocalDB
- Visual Studio 2022 o VS Code (opcional)

## 🔧 Configuración

### 1. Restaurar Dependencias

```powershell
cd "c:\Users\svsil\Desktop\Silvas-Indrustries\Sistema Gimnasios\SistemaReservaGimnasioBK"
dotnet restore
```

### 2. Configurar Base de Datos (SQLite)

El proyecto ha sido configurado para usar **SQLite** para facilitar el desarrollo local sin necesidad de instalar SQL Server.

La cadena de conexión en `appsettings.json` es:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

La base de datos `app.db` se creará automáticamente en la carpeta del proyecto WebAPI al aplicar las migraciones.

### 3. Crear Base de Datos con Migraciones

> **Nota:** Si el comando `dotnet` no está en tu PATH, deberás usar la ruta completa (ej: `C:\Users\svsil\.dotnet\dotnet.exe`).

```powershell
# Instalar herramienta EF Core (si es necesario)
& "C:\Users\svsil\.dotnet\dotnet.exe" tool install --global dotnet-ef

# Crear migración inicial (ya ejecutado)
# & "C:\Users\svsil\.dotnet\tools\dotnet-ef.exe" migrations add InitialCreate --project SistemaReservaGimnasioBK.Infrastructure --startup-project SistemaReservaGimnasioBK

# Aplicar migración a la base de datos (ya ejecutado)
# & "C:\Users\svsil\.dotnet\tools\dotnet-ef.exe" database update --project SistemaReservaGimnasioBK.Infrastructure --startup-project SistemaReservaGimnasioBK
```

## ▶️ Ejecutar la Aplicación

```powershell
& "C:\Users\svsil\.dotnet\dotnet.exe" run --project SistemaReservaGimnasioBK
```

La API estará disponible en:
- **Swagger UI**: `https://localhost:5001` (o puerto asignado)
- **Health Check**: `https://localhost:5001/health`

## 📚 Endpoints Principales

### Espacios

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/spaces` | Listar espacios activos |
| GET | `/api/spaces/{id}` | Obtener espacio por ID |
| POST | `/api/spaces` | Crear nuevo espacio |
| DELETE | `/api/spaces/{id}` | Desactivar espacio |
| GET | `/api/spaces/available?startDateTime=...&endDateTime=...` | **Obtener espacios disponibles** |

### Reservas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/bookings` | Crear reserva |
| GET | `/api/bookings/{id}` | Obtener reserva por ID |
| GET | `/api/bookings/user/{userId}` | Reservas de un usuario |
| DELETE | `/api/bookings/{id}` | Cancelar reserva |

## 🧪 Ejemplo de Uso

### 1. Crear un Espacio

```http
POST /api/spaces
Content-Type: application/json

{
  "name": "Sala de Reuniones A",
  "description": "Sala con capacidad para 8 personas",
  "capacity": 8,
  "type": 2  // MeetingRoom
}
```

### 2. Consultar Espacios Disponibles

```http
GET /api/spaces/available?startDateTime=2026-02-20T09:00:00Z&endDateTime=2026-02-20T11:00:00Z
```

### 3. Crear una Reserva

```http
POST /api/bookings
Content-Type: application/json

{
  "spaceId": "guid-del-espacio",
  "userId": "guid-del-usuario",
  "startDateTime": "2026-02-20T09:00:00Z",
  "endDateTime": "2026-02-20T11:00:00Z",
  "notes": "Reunión de equipo"
}
```

**Respuesta exitosa (201 Created):**

```json
{
  "id": "nuevo-guid",
  "spaceId": "guid-del-espacio",
  "spaceName": "Sala de Reuniones A",
  "userId": "guid-del-usuario",
  "startDateTime": "2026-02-20T09:00:00Z",
  "endDateTime": "2026-02-20T11:00:00Z",
  "status": "Confirmed",
  "notes": "Reunión de equipo",
  "createdAt": "2026-02-16T23:00:00Z"
}
```

**Error si el espacio está ocupado (400 Bad Request):**

```json
{
  "message": "El espacio no está disponible en el período solicitado (20/02/2026 9:00 - 20/02/2026 11:00)"
}
```

## 🔑 Conceptos DDD Implementados

### Value Object con Lógica de Negocio

`ReservationPeriod` encapsula el período de reserva y valida solapamientos:

```csharp
var period = ReservationPeriod.Create(startDate, endDate);
bool overlaps = period.Overlaps(otherPeriod); // Detecta conflictos
```

### Domain Service - Invariante de Negocio

La regla *"No se puede reservar un espacio ocupado"* se valida en `BookingDomainService`:

```csharp
await _bookingDomainService.ValidateBookingCreationAsync(spaceId, period);
// Lanza excepción si hay conflicto
```

### Persistencia de Value Objects

`ReservationPeriod` se persiste en la tabla `Bookings` (no en tabla separada):

```csharp
// Configuración EF Core
builder.OwnsOne(b => b.Period, period => {
    period.Property(p => p.StartDateTime).HasColumnName("StartDateTime");
    period.Property(p => p.EndDateTime).HasColumnName("EndDateTime");
});
```

## 📂 Estructura de Archivos Relevantes

### Domain Layer
- `ValueObjects/ReservationPeriod.cs` - **Value Object clave con lógica de solapamiento**
- `Entities/Space.cs`, `Entities/Booking.cs` - Aggregate Roots
- `Services/BookingDomainService.cs` - **Validación de invariante de negocio**
- `Repositories/IBookingRepository.cs` - Definición de contrato

### Infrastructure Layer
- `Persistence/ApplicationDbContext.cs` - Configuración EF Core
- `Persistence/Configurations/BookingConfiguration.cs` - **Mapeo de Value Object**
- `Repositories/BookingRepository.cs` - **Query de disponibilidad**

### Application Layer
- `CommandHandlers/CreateBookingCommandHandler.cs` - **Flujo de creación de reserva**
- `Validators/CreateBookingCommandValidator.cs` - FluentValidation

### WebAPI Layer
- `Program.cs` - **Configuración de Dependency Injection**
- `Controllers/BookingsController.cs` - API REST

## 🛠️ Tecnologías Utilizadas

- .NET 8
- Entity Framework Core 8.0
- FluentValidation 11.9
- Swashbuckle (Swagger) 6.6
- SQL Server

## 📖 Documentación Adicional

Ver [`walkthrough.md`](file:///C:/Users/svsil/.gemini/antigravity/brain/533d5750-6ac2-409e-a39e-49d9908547d9/walkthrough.md) para documentación detallada de la arquitectura y decisiones de diseño.

## 👨‍💻 Autor

Sistema creado por un Arquitecto de Software Senior especializado en .NET y DDD.

## 📄 Licencia

Este proyecto es un ejemplo educativo de arquitectura DDD con .NET 8.
