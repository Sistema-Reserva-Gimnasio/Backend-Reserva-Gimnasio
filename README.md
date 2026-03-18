# SistemaReservaGimnasioBK (Backend)

API REST para gestionar **espacios** (salas, áreas, canchas, etc.) y sus **reservas**.
La intención es que cualquiera pueda clonar el repo, levantarlo rápido y entender cómo están separadas las responsabilidades usando **DDD + CQRS**.

## Qué incluye

- Endpoints para **Espacios**: listar, crear, desactivar y consultar disponibilidad
- Endpoints para **Reservas**: crear, consultar, listar por usuario y cancelar
- Reglas de negocio en dominio (por ejemplo: evitar solapamientos)
- Validación con **FluentValidation**
- Persistencia con **EF Core**
- **Swagger UI** para probar la API
- Health check simple: `GET /health`

## Stack

- .NET 8 (ASP.NET Core)
- Entity Framework Core
- SQLite (por defecto en desarrollo)
- FluentValidation
- Swagger (Swashbuckle)

## Arquitectura (DDD)

```
SistemaReservaGimnasioBK.Domain/          # Dominio: entidades, value objects, reglas
SistemaReservaGimnasioBK.Application/     # Casos de uso: comandos/queries + validaciones
SistemaReservaGimnasioBK.Infrastructure/  # Infra: EF Core, repositorios
SistemaReservaGimnasioBK/                # WebAPI: controllers y composición (DI)
```

## Requisitos

- .NET 8 SDK

## Base de datos (SQLite)

La conexión está en `SistemaReservaGimnasioBK\appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

> Nota: `app.db` (SQLite) se creará cuando apliques migraciones.

## Levantar el backend

Desde la carpeta donde está la solución (`SistemaReservaGimnasioBK.sln`):

```powershell
# Restaurar
 dotnet restore

# (Opcional) Instalar EF CLI
 dotnet tool install --global dotnet-ef

# Crear migración inicial (si aún no existe en tu copia)
 dotnet ef migrations add InitialCreate --project SistemaReservaGimnasioBK.Infrastructure --startup-project SistemaReservaGimnasioBK

# Aplicar a la base
 dotnet ef database update --project SistemaReservaGimnasioBK.Infrastructure --startup-project SistemaReservaGimnasioBK

# Ejecutar API
 dotnet run --project SistemaReservaGimnasioBK
```

## URLs útiles (Development)

Según `SistemaReservaGimnasioBK\Properties\launchSettings.json`:

- HTTP: `http://localhost:5204`
- HTTPS: `https://localhost:7288`

Swagger UI está servido en la **raíz**:

- `http://localhost:5204/`
- `https://localhost:7288/`

Otros:

- OpenAPI JSON: `/swagger/v1/swagger.json`
- Health: `/health`

## Endpoints principales

### Espacios

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/spaces` | Listar espacios activos |
| GET | `/api/spaces/{id}` | Obtener espacio por ID |
| POST | `/api/spaces` | Crear espacio |
| DELETE | `/api/spaces/{id}` | Desactivar espacio |
| GET | `/api/spaces/available?startDateTime=...&endDateTime=...` | Consultar disponibilidad |

### Reservas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/bookings` | Crear reserva |
| GET | `/api/bookings/{id}` | Obtener reserva por ID |
| GET | `/api/bookings/user/{userId}` | Reservas de un usuario |
| DELETE | `/api/bookings/{id}` | Cancelar reserva |

## Ejemplos rápidos

Crear un espacio:

```http
POST /api/spaces
Content-Type: application/json

{
  "name": "Sala A",
  "description": "Sala con proyector",
  "capacity": 8,
  "type": 2
}
```

Consultar disponibilidad:

```http
GET /api/spaces/available?startDateTime=2026-02-20T09:00:00Z&endDateTime=2026-02-20T11:00:00Z
```

## Notas de dominio (rápidas)

- `ReservationPeriod` es un **Value Object** que encapsula el período y detecta solapamientos.
- La regla “no se puede reservar un espacio ocupado” se valida con un **Domain Service**.

## Problemas comunes

- Si `http://localhost:5204/swagger` no abre: Swagger UI está en `http://localhost:5204/`.
- Si EF Core falla con la cadena de conexión: asegúrate de usar una cadena válida para SQLite (por ejemplo `Data Source=app.db`).
