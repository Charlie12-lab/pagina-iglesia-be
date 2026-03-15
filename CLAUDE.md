# IglesiaNet — Backend Agent

## Proyecto
Plataforma web multi-iglesia para un país. Permite gestionar iglesias, eventos con inscripción y blogs.

## Arquitectura
DDD (Domain-Driven Design) con 4 proyectos en `backend/`:

```
IglesiaNet.Domain/          ← Pure C#, sin dependencias externas
IglesiaNet.Application/     ← Application Services, DTOs, port interfaces
IglesiaNet.Infrastructure/  ← EF Core, MongoDB, JWT, BCrypt
IglesiaNet.API/             ← Controllers finos, Swagger, DI, Program.cs
```

Dependencias: `API → Application → Domain ← Infrastructure`

## Stack
- .NET 8 / ASP.NET Core 8
- SQL Server + EF Core 8 (Fluent API, sin DataAnnotations en Domain)
- MongoDB + MongoDB.Driver (BlogPost)
- JWT Bearer (8h) + BCrypt.Net-Next
- FluentValidation (instalado, validators pendientes)

## Aggregates y Value Objects

| Aggregate | Persistence | Value Objects |
|-----------|-------------|---------------|
| Church    | SQL Server  | Email         |
| Event     | SQL Server  | EventSchedule, EventCapacity |
| EventRegistration | SQL (owned by Event) | — |
| BlogPost  | MongoDB     | BlogPublication |
| User      | SQL Server  | — |

### Reglas de dominio importantes
- `Event.Register()` valida capacidad y lanza `DomainException` si está lleno o sin inscripciones
- `EventSchedule`: EndDate >= StartDate (invariante)
- `User`: ChurchAdmin **debe** tener ChurchId; SuperAdmin tiene ChurchId null
- `Email`: validación regex en Value Object, shared en `Domain/Shared/`

## Roles
- `SuperAdmin` — CRUD completo de todo
- `ChurchAdmin` — solo gestiona eventos y blogs de su propia iglesia (churchId en JWT claim)

## Archivos clave
- `IglesiaNet.API/Program.cs` — DI, JWT, CORS, auto-migration al iniciar
- `IglesiaNet.API/appsettings.json` — connection strings y JWT config
- `IglesiaNet.Infrastructure/Persistence/SQL/AppDbContext.cs` — DbContext EF Core
- `IglesiaNet.Infrastructure/Persistence/SQL/Migrations/` — migraciones EF
- `IglesiaNet.Infrastructure/Persistence/MongoDB/MongoDbContext.cs` — singleton MongoDB
- `IglesiaNet.Domain/Shared/` — Entity, ValueObject, DomainException, Email

## Comandos
```bash
# Correr backend (desde raíz del repo)
dotnet run --project IglesiaNet.API

# Nueva migración EF
dotnet ef migrations add NombreMigracion \
  --project IglesiaNet.Infrastructure \
  --startup-project IglesiaNet.API \
  --output-dir Persistence/SQL/Migrations

# Build completo
dotnet build
```

## Requisitos para correr
1. SQL Server local (la DB se crea automáticamente con EF al iniciar)
2. MongoDB en `mongodb://localhost:27017`
3. `appsettings.json` con:
   - `ConnectionStrings:SqlServer` — connection string SQL Server
   - `ConnectionStrings:MongoDb` — connection string MongoDB
   - `MongoDb:DatabaseName` — nombre de la DB Mongo
   - `Jwt:Secret` — mínimo 32 caracteres
   - `Jwt:Issuer` y `Jwt:Audience`

## Primer SuperAdmin
No hay seed. Insertar directamente en SQL después de la primera migración:
```sql
INSERT INTO Users (Username, Email, PasswordHash, Role, IsActive, ChurchId)
VALUES ('admin', 'admin@iglesianet.com',
  '$2a$11$rBnKJEJQzGnzVZw0i.ZfWOnrdB7MhBvP3PYAQaOlQBhvXmOZxQmLe',
  'SuperAdmin', 1, NULL)
-- Contraseña: Admin123!
```

## GitHub
Repo: https://github.com/Charlie12-lab/pagina-iglesia-be

## Pendiente
- Escribir validators FluentValidation (uno por request en Application)
- Tests unitarios de dominio
