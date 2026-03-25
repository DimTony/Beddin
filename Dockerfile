# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files for efficient layer caching
COPY ["src/api/Beddin.API/Beddin.API.csproj", "src/api/Beddin.API/"]
COPY ["src/api/Beddin.Application/Beddin.Application.csproj", "src/api/Beddin.Application/"]
COPY ["src/api/Beddin.Domain/Beddin.Domain.csproj", "src/api/Beddin.Domain/"]
COPY ["src/api/Beddin.Infrastructure/Beddin.Infrastructure.csproj", "src/api/Beddin.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/api/Beddin.API/Beddin.API.csproj"

# Copy source code
COPY src/api/ src/api/

# Build
WORKDIR "/src/src/api/Beddin.API"
RUN dotnet build "Beddin.API.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Publish Stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Beddin.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --no-build \
    /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Install curl for health checks
RUN apt-get update && \
    apt-get install -y curl && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Create non-root user for security (AWS ECS best practice)
RUN groupadd -r beddingroup --gid=1001 && \
    useradd -r -g beddingroup --uid=1001 --create-home beddinuser

# Copy published files
COPY --from=publish --chown=beddinuser:beddingroup /app/publish .

# Switch to non-root user
USER beddinuser

# Expose port (ECS will dynamically map this)
EXPOSE 8080

# Environment variables for production
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Health check for ECS target group
# Note: You should implement a /health endpoint in your API
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/api/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Beddin.API.dll"]