# SportsLeague API

API REST para la gestión de torneos de fútbol (Liga BetPlay). Construida con .NET 10 + Entity Framework Core + SQL Server.

## Tecnologías

- **.NET 10** - Framework base
- **ASP.NET Core Web API** - Capa de presentación
- **Entity Framework Core 8** - ORM y acceso a datos
- **SQL Server** - Base de datos relacional
- **Swagger / Swashbuckle** - Documentación interactiva de la API
- **AutoMapper** - Mapeo de entidades a DTOs
- **Repository Pattern + Service Layer** - Arquitectura en capas

## Estructura del proyecto

```
SportsLeague.slnx
├── SportsLeague.API          # Web API (Controllers, DTOs, Mappings)
├── SportsLeague.Domain       # Lógica de negocio (Entities, Services, Interfaces)
└── SportsLeague.DataAccess   # Persistencia (DbContext, Repositories, Migrations, Seeders)
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (local o remoto)

## Configuración

1. Clona el repositorio
2. Actualiza la cadena de conexión en `SportsLeague.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SportsLeagueDb;Trusted_Connection=true;TrustServerCertificate=true;"
}
```
3. Ejecuta la aplicación:
```bash
cd SportsLeague.API
dotnet run
```

La primera vez, `MigrateAsync()` creará la BD aplicando las migraciones y el `DataSeeder` la poblará automáticamente con 20 equipos, 80 jugadores, 4 árbitros y un torneo.

## Endpoints principales

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/team` | Listar equipos |
| GET | `/api/team/{id}` | Equipo por ID |
| POST | `/api/team` | Crear equipo |
| GET | `/api/player` | Listar jugadores |
| GET | `/api/player/{id}` | Jugador por ID |
| GET | `/api/player/team/{teamId}` | Jugadores por equipo |
| GET | `/api/tournament` | Listar torneos |
| POST | `/api/tournament/{id}/teams` | Inscribir equipo en torneo |
| GET | `/api/match/tournament/{id}` | Partidos de un torneo |
| POST | `/api/match` | Crear partido |
| POST | `/api/match/{id}/result` | Registrar resultado |
| GET | `/api/standings` | Tabla de posiciones |
| GET | `/api/stats/scorers` | Goleadores |
| GET | `/api/stats/cards` | Estadísticas de tarjetas |

> La documentación interactiva (Swagger) está disponible en `/swagger` al ejecutar en entorno Development.
