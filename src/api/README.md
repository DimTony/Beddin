# Beddin

A DDD-based property management platform built with .NET 8 and Next.js.

## Architecture

- **Backend**: .NET 8 Web API with Clean Architecture (DDD principles)
- **Frontend**: Next.js with TypeScript
- **Database**: PostgreSQL 16 with PostGIS
- **Cache**: Redis 7
- **Observability**: OpenTelemetry (OTLP)

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (8.0 or later)
- [Node.js](https://nodejs.org/) (18.x or later) and npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for running PostgreSQL and Redis)
- [Git](https://git-scm.com/downloads)

### Recommended Tools

- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Azure Data Studio](https://azure.microsoft.com/products/data-studio/) or [pgAdmin](https://www.pgadmin.org/) (for database management)
- [Redis Insight](https://redis.io/insight/) (for Redis management)

## Getting Started

### 1. Clone the Repository

### 2. Start Infrastructure Services

Start PostgreSQL and Redis using Docker Compose:

````````yaml
version: '3.8'

services:
  db:
    image: postgres:16
    ports:
      - "5432:5432"
    environment:
      POSTGRES_DB: beddin
      POSTGRES_USER: beddin_user
      POSTGRES_PASSWORD: secure_password
  redis:
    image: redis:7
    ports:
      - "6379:6379"
````````

Verify services are running:

```
docker-compose ps
```

### 3. Configure the API

Create or update `src/api/Beddin.API/appsettings.Development.json`:

### 4. Run Database Migrations

Navigate to the API project and apply migrations:

`
cd src/api/Beddin.API dotnet ef database update --project ../Beddin.Infrastructure
`

### 5. Run the Backend API

````````bash
cd src/api/Beddin.API
dotnet run
````````

Visit `https://localhost:7166/weatherforecast` to verify the API is running.

The API will be available at `https://localhost:7071` (or the configured port).

### 6. Run the Frontend

Open a new terminal and navigate to the web project:

The frontend will be available at `http://localhost:3000`.

### 7. Stop Infrastructure Services

````````bash
docker-compose down
````````

This command stops and removes the containers for the services defined in your `docker-compose.yml` file.
Make sure to run this command in the same directory where your `docker-compose.yml` file is located.


## Project Structure

## Development

### Entity Framework Core Commands

`dotnet ef migrations add MigrationName --project src/api/Beddin.Infrastructure --startup-project src/api/Beddin.API`

`dotnet ef database update --project src/api/Beddin.Infrastructure --startup-project src/api/Beddin.API`

## Configuration

### Connection Strings

The default connection string for local development:

`Host=localhost;Port=5432;Database=beddin;Username=beddin_user;Password=beddin_dev_password`

### Redis Configuration

Redis is available at `localhost:6379` with no password for local development.

## Troubleshooting

### PostgreSQL Connection Issues

1. Ensure Docker containers are running: `docker-compose ps`
2. Check PostgreSQL logs: `docker logs beddin-postgres`
3. Verify connection string in `appsettings.Development.json`

### Port Conflicts

If ports 5432 or 6379 are already in use, modify the port mappings in `docker-compose.yml`:

## Contributing

1. Create a feature branch from `develop`
2. Make your changes following the project's coding standards
3. Ensure all tests pass
4. Submit a pull request

## License

See [LICENSE](LICENSE) file for details.

## Support

For issues and questions, please use the [GitHub Issues](https://github.com/DimTony/Beddin/issues) page.